using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Synesthesia.App.Audio;
using Synesthesia.App.Models;
using Synesthesia.App.Visuals;

namespace Synesthesia.App
{
    public partial class MainWindow : Window
    {
        private readonly MusicPlayer musicPlayer;
        private readonly Playlist playlist;
        private readonly VisualEngine visualEngine;
        private readonly DispatcherTimer positionTimer;
        private readonly DispatcherTimer visualizerTimer;
        private bool isDraggingSlider = false;
        private bool isShuffleEnabled = false;
        private bool isRepeatEnabled = false;
        private Window? fullscreenWindow;

        public ObservableCollection<Track> PlaylistItems { get; } = new();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                musicPlayer = new MusicPlayer();
                playlist = new Playlist();
                visualEngine = new VisualEngine(VisualizerCanvas);

                // Configurar timers
                positionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                positionTimer.Tick += PositionTimer_Tick;

                visualizerTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                visualizerTimer.Tick += VisualizerTimer_Tick;
                visualizerTimer.Start();

                // Configurar eventos
                musicPlayer.PlaybackStateChanged += MusicPlayer_PlaybackStateChanged;
                musicPlayer.PlaybackStopped += MusicPlayer_PlaybackStopped;
                playlist.TrackChanged += Playlist_TrackChanged;

                // Configurar DataContext
                PlaylistListBox.ItemsSource = PlaylistItems;

                // Configurar visualizadores
                SetupVisualizers();

                Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing main window: {ex.Message}\n\nStack trace: {ex.StackTrace}", 
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Adicionar algumas faixas de exemplo (opcional)
            // LoadSampleTracks();
        }

        private void SetupVisualizers()
        {
            visualEngine.AddVisualizer("Bars", new BarsVisualizer(VisualizerCanvas));
            visualEngine.AddVisualizer("Wave", new WaveVisualizer(VisualizerCanvas));
            visualEngine.AddVisualizer("Kaleidoscope", new KaleidoVisualizer(VisualizerCanvas));
            visualEngine.AddVisualizer("Spectrum", new SpectrumVisualizer(VisualizerCanvas));
            visualEngine.AddVisualizer("Particle Flow", new ParticleFlowVisualizer(VisualizerCanvas));
            
            visualEngine.SetVisualizer("Bars");
        }

        #region Title Bar Events

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            MaximizeButton.Content = WindowState == WindowState.Maximized ? "🗗" : "🗖";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Playlist Events

        private void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio files (*.mp3;*.wav;*.flac;*.m4a)|*.mp3;*.wav;*.flac;*.m4a|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    AddTrackToPlaylist(filename);
                }
            }
        }

        private void AddTrackToPlaylist(string filePath)
        {
            try
            {
                var track = new Track(filePath);
                
                // Extrair metadados básicos do arquivo (nome sem extensão)
                track.Title = Path.GetFileNameWithoutExtension(filePath);
                track.Artist = "Unknown Artist";

                playlist.AddTrack(track);
                PlaylistItems.Add(track);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding track: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistListBox.SelectedItem is Track selectedTrack)
            {
                await LoadAndPlayTrack(selectedTrack);
            }
        }

        #endregion

        #region Media Control Events

        private async void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.PlaybackState == PlaybackState.Playing)
            {
                musicPlayer.Pause();
            }
            else if (musicPlayer.PlaybackState == PlaybackState.Paused)
            {
                musicPlayer.Play();
            }
            else if (playlist.Tracks.Any())
            {
                var track = playlist.CurrentTrack ?? playlist.GetNextTrack();
                if (track != null)
                {
                    await LoadAndPlayTrack(track);
                }
            }
        }

        private async void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            var previousTrack = playlist.GetPreviousTrack();
            if (previousTrack != null)
            {
                await LoadAndPlayTrack(previousTrack);
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var nextTrack = playlist.GetNextTrack();
            if (nextTrack != null)
            {
                await LoadAndPlayTrack(nextTrack);
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            isShuffleEnabled = !isShuffleEnabled;
            playlist.IsShuffleEnabled = isShuffleEnabled;
            ShuffleButton.Opacity = isShuffleEnabled ? 1.0 : 0.5;
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            isRepeatEnabled = !isRepeatEnabled;
            playlist.IsRepeatEnabled = isRepeatEnabled;
            RepeatButton.Opacity = isRepeatEnabled ? 1.0 : 0.5;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicPlayer != null)
            {
                musicPlayer.Volume = (float)(e.NewValue / 100.0);
            }
        }

        #endregion

        #region Progress Control Events

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isDraggingSlider && musicPlayer != null)
            {
                var newPosition = TimeSpan.FromSeconds(e.NewValue);
                musicPlayer.Position = newPosition;
            }
        }

        private void ProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDraggingSlider = true;
        }

        private void ProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraggingSlider = false;
            var newPosition = TimeSpan.FromSeconds(ProgressSlider.Value);
            musicPlayer.Position = newPosition;
        }

        #endregion

        #region Visualizer Events

        private void VisualizerCard_Checked(object sender, RoutedEventArgs e)
        {
            if (visualEngine == null || sender is not System.Windows.Controls.Primitives.ToggleButton checkedCard)
                return;

            // Uncheck all other visualizer cards
            var allCards = new[] { BarsVisualizerCard, WaveVisualizerCard, KaleidoVisualizerCard, 
                                   SpectrumVisualizerCard, ParticleVisualizerCard };
            
            foreach (var card in allCards)
            {
                if (card != checkedCard && card != null)
                    card.IsChecked = false;
            }

            // Set the visualizer based on the card content
            string? visualizerName = checkedCard.Content?.ToString();
            if (!string.IsNullOrEmpty(visualizerName))
            {
                visualEngine.SetVisualizer(visualizerName);
            }
        }

        private void FullscreenVisualizerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OpenFullscreenVisualizer();
        }

        private void FullscreenVisualizerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CloseFullscreenVisualizer();
        }

        #endregion

        #region Timer Events

        private void PositionTimer_Tick(object? sender, EventArgs e)
        {
            if (!isDraggingSlider)
            {
                ProgressSlider.Value = musicPlayer.Position.TotalSeconds;
                CurrentTimeText.Text = FormatTime(musicPlayer.Position);
            }
        }

        private void VisualizerTimer_Tick(object? sender, EventArgs e)
        {
            if (musicPlayer.PlaybackState == PlaybackState.Playing)
            {
                var fftData = musicPlayer.GetFFTData();
                visualEngine.Render(fftData);
            }
        }

        #endregion

        #region Music Player Events

        private void MusicPlayer_PlaybackStateChanged(object? sender, MusicPlayerStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (e.NewState)
                {
                    case PlaybackState.Playing:
                        PlayPauseButton.Content = "⏸";
                        positionTimer.Start();
                        break;
                    case PlaybackState.Paused:
                        PlayPauseButton.Content = "▶";
                        positionTimer.Stop();
                        break;
                    case PlaybackState.Stopped:
                        PlayPauseButton.Content = "▶";
                        positionTimer.Stop();
                        ProgressSlider.Value = 0;
                        CurrentTimeText.Text = "0:00";
                        break;
                }
            });
        }

        private async void MusicPlayer_PlaybackStopped(object? sender, EventArgs e)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                var nextTrack = playlist.GetNextTrack();
                if (nextTrack != null)
                {
                    await LoadAndPlayTrack(nextTrack);
                }
            });
        }

        private void Playlist_TrackChanged(object? sender, TrackChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                NowPlayingTitle.Text = e.Track.Title;
                NowPlayingArtist.Text = e.Track.Artist;
                
                // Atualizar seleção na playlist
                PlaylistListBox.SelectedItem = e.Track;
            });
        }

        #endregion

        #region Helper Methods

        private async Task LoadAndPlayTrack(Track track)
        {
            try
            {
                var success = await musicPlayer.LoadTrackAsync(track);
                if (success)
                {
                    playlist.SetCurrentTrack(track);
                    ProgressSlider.Maximum = musicPlayer.Duration.TotalSeconds;
                    TotalTimeText.Text = FormatTime(musicPlayer.Duration);
                    musicPlayer.Play();
                }
                else
                {
                    MessageBox.Show($"Failed to load track: {track.Title}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing track: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string FormatTime(TimeSpan time)
        {
            return $"{(int)time.TotalMinutes}:{time.Seconds:D2}";
        }

        private void OpenFullscreenVisualizer()
        {
            if (fullscreenWindow == null)
            {
                fullscreenWindow = new Window
                {
                    WindowStyle = WindowStyle.None,
                    WindowState = WindowState.Maximized,
                    Background = System.Windows.Media.Brushes.Black,
                    Topmost = true
                };

                var fullscreenCanvas = new Canvas();
                fullscreenWindow.Content = fullscreenCanvas;

                // Criar um novo VisualEngine para tela cheia
                var fullscreenVisualEngine = new VisualEngine(fullscreenCanvas);
                fullscreenVisualEngine.AddVisualizer("Bars", new BarsVisualizer(fullscreenCanvas));
                fullscreenVisualEngine.AddVisualizer("Wave", new WaveVisualizer(fullscreenCanvas));
                fullscreenVisualEngine.AddVisualizer("Kaleidoscope", new KaleidoVisualizer(fullscreenCanvas));
                fullscreenVisualEngine.AddVisualizer("Spectrum", new SpectrumVisualizer(fullscreenCanvas));
                fullscreenVisualEngine.AddVisualizer("Particle Flow", new ParticleFlowVisualizer(fullscreenCanvas));

                // Get the currently selected visualizer from the cards
                string currentVisualizer = "Bars"; // default
                if (BarsVisualizerCard?.IsChecked == true) currentVisualizer = "Bars";
                else if (WaveVisualizerCard?.IsChecked == true) currentVisualizer = "Wave";
                else if (KaleidoVisualizerCard?.IsChecked == true) currentVisualizer = "Kaleidoscope";
                else if (SpectrumVisualizerCard?.IsChecked == true) currentVisualizer = "Spectrum";
                else if (ParticleVisualizerCard?.IsChecked == true) currentVisualizer = "Particle Flow";
                
                fullscreenVisualEngine.SetVisualizer(currentVisualizer);

                // Timer para atualizar visualizador em tela cheia
                var fullscreenTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                fullscreenTimer.Tick += (s, e) =>
                {
                    if (musicPlayer.PlaybackState == PlaybackState.Playing)
                    {
                        var fftData = musicPlayer.GetFFTData();
                        fullscreenVisualEngine.Render(fftData);
                    }
                };
                fullscreenTimer.Start();

                fullscreenWindow.Closed += (s, e) =>
                {
                    fullscreenTimer.Stop();
                    fullscreenWindow = null;
                    FullscreenVisualizerCheckBox.IsChecked = false;
                };

                fullscreenWindow.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Escape)
                    {
                        CloseFullscreenVisualizer();
                    }
                };

                fullscreenWindow.Show();
            }
        }

        private void CloseFullscreenVisualizer()
        {
            fullscreenWindow?.Close();
            fullscreenWindow = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            positionTimer?.Stop();
            visualizerTimer?.Stop();
            musicPlayer?.Dispose();
            fullscreenWindow?.Close();
            base.OnClosed(e);
        }

        #endregion
    }
}

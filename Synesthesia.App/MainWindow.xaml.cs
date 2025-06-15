using Microsoft.Win32;
using Synesthesia.App.Audio;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using NAudio.Wave;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Synesthesia.App
{
    public partial class MainWindow : Window
    {
        private readonly AudioPlayer audioPlayer;
        private readonly DispatcherTimer positionTimer;
        private readonly DispatcherTimer visualizerTimer;

        public MainWindow()
        {
            InitializeComponent();

            audioPlayer = new AudioPlayer();
            audioPlayer.PlaybackStateChanged += OnPlaybackStateChanged;
            audioPlayer.PlaybackStopped += OnPlaybackStopped;

            positionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            positionTimer.Tick += UpdatePosition;

            visualizerTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33) // ~30 FPS
            };
            visualizerTimer.Tick += UpdateVisualizer;
        }

        private void OnPlaybackStateChanged(object? sender, PlaybackStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (e.State)
                {
                    case PlaybackState.Playing:
                        playButton.Content = "⏸";
                        positionTimer.Start();
                        visualizerTimer.Start();
                        break;
                    case PlaybackState.Paused:
                    case PlaybackState.Stopped:
                        playButton.Content = "▶";
                        positionTimer.Stop();
                        visualizerTimer.Stop();
                        break;
                }
            });
        }

        private void OnPlaybackStopped(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                positionSlider.Value = 0;
                currentTimeLabel.Content = "00:00";
                visualizerTimer.Stop();
                visualizerCanvas.Children.Clear();
            });
        }

        private void UpdatePosition(object? sender, EventArgs e)
        {
            if (audioPlayer.IsLoaded)
            {
                var current = audioPlayer.CurrentTime;
                var total = audioPlayer.TotalTime;

                if (total.TotalSeconds > 0)
                {
                    positionSlider.Value = current.TotalSeconds / total.TotalSeconds * 100;
                }

                currentTimeLabel.Content = $"{current:mm\\:ss}";
            }
        }

        private void UpdateVisualizer(object? sender, EventArgs e)
        {
            if (audioPlayer.Analyzer?.SpectrumData == null)
                return;

            var spectrum = audioPlayer.Analyzer.SpectrumData;
            if (spectrum.Length == 0 || visualizerCanvas.ActualWidth == 0)
                return;

            visualizerCanvas.Children.Clear();

            int barCount = spectrum.Length;
            double canvasWidth = visualizerCanvas.ActualWidth;
            double canvasHeight = visualizerCanvas.ActualHeight;
            double barWidth = canvasWidth / barCount;

            for (int i = 0; i < barCount; i++)
            {
                double magnitude = Math.Clamp(spectrum[i] * 10, 2, canvasHeight);
                var bar = new Rectangle
                {
                    Width = barWidth - 1,
                    Height = magnitude,
                    Fill = Brushes.LimeGreen
                };

                System.Windows.Controls.Canvas.SetLeft(bar, i * barWidth);
                System.Windows.Controls.Canvas.SetTop(bar, canvasHeight - magnitude);
                visualizerCanvas.Children.Add(bar);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de áudio|*.mp3;*.wav;*.flac|Todos os arquivos|*.*",
                Title = "Selecionar arquivo de áudio"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    audioPlayer.Load(openFileDialog.FileName);

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    fileNameLabel.Content = fileName;
                    totalTimeLabel.Content = audioPlayer.TotalTime.ToString(@"mm\:ss");
                    positionSlider.Value = 0;

                    playButton.IsEnabled = true;
                    stopButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar arquivo: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (audioPlayer.IsPlaying)
                {
                    audioPlayer.Pause();
                }
                else
                {
                    audioPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na reprodução: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Stop();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioPlayer != null)
            {
                audioPlayer.Volume = (float)(e.NewValue / 100);
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (positionSlider.IsMouseCaptureWithin && audioPlayer.IsLoaded)
            {
                audioPlayer.SetPosition(e.NewValue / 100);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            audioPlayer?.Dispose();
            positionTimer?.Stop();
            visualizerTimer?.Stop();
            base.OnClosed(e);
        }
    }
}

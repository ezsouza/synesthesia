using NAudio.Wave;
using System;
using System.IO;

namespace Synesthesia.App.Audio
{
    public class AudioPlayer : IDisposable
    {
        private WaveOutEvent? outputDevice;
        private AudioFileReader? audioFile;
        private bool isDisposed = false;

        public event EventHandler<PlaybackStateChangedEventArgs>? PlaybackStateChanged;
        public event EventHandler<EventArgs>? PlaybackStopped;

        // Propriedades públicas
        public bool IsPlaying => outputDevice?.PlaybackState == PlaybackState.Playing;
        public bool IsPaused => outputDevice?.PlaybackState == PlaybackState.Paused;
        public bool IsLoaded => audioFile != null;
        
        public TimeSpan CurrentTime => audioFile?.CurrentTime ?? TimeSpan.Zero;
        public TimeSpan TotalTime => audioFile?.TotalTime ?? TimeSpan.Zero;
        
        public float Volume 
        { 
            get => outputDevice?.Volume ?? 0f;
            set 
            {
                if (outputDevice != null)
                    outputDevice.Volume = Math.Clamp(value, 0f, 1f);
            }
        }

        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

            var extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".mp3" && extension != ".wav" && extension != ".flac")
                throw new NotSupportedException($"Formato não suportado: {extension}");

            Dispose(); // Limpa recursos anteriores
            
            try
            {
                audioFile = new AudioFileReader(filePath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                
                // Configurar eventos
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            catch (Exception ex)
            {
                Dispose();
                throw new InvalidOperationException($"Erro ao carregar arquivo: {ex.Message}", ex);
            }
        }

        public void Play()
        {
            if (outputDevice == null || audioFile == null)
                throw new InvalidOperationException("Nenhum arquivo carregado");

            outputDevice.Play();
            PlaybackStateChanged?.Invoke(this, new PlaybackStateChangedEventArgs(PlaybackState.Playing));
        }

        public void Pause()
        {
            if (outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
                PlaybackStateChanged?.Invoke(this, new PlaybackStateChangedEventArgs(PlaybackState.Paused));
            }
        }

        public void Stop()
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                if (audioFile != null)
                    audioFile.Position = 0; // Volta para o início
                PlaybackStateChanged?.Invoke(this, new PlaybackStateChangedEventArgs(PlaybackState.Stopped));
            }
        }

        public void SetPosition(TimeSpan position)
        {
            if (audioFile != null)
            {
                audioFile.CurrentTime = position;
            }
        }

        public void SetPosition(double percentage)
        {
            if (audioFile != null && percentage >= 0 && percentage <= 1)
            {
                audioFile.CurrentTime = TimeSpan.FromMilliseconds(audioFile.TotalTime.TotalMilliseconds * percentage);
            }
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            PlaybackStopped?.Invoke(this, EventArgs.Empty);
            PlaybackStateChanged?.Invoke(this, new PlaybackStateChangedEventArgs(PlaybackState.Stopped));
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                outputDevice?.Stop();
                outputDevice?.Dispose();
                audioFile?.Dispose();
                outputDevice = null;
                audioFile = null;
                isDisposed = true;
            }
        }
    }

    public class PlaybackStateChangedEventArgs : EventArgs
    {
        public PlaybackState State { get; }
        
        public PlaybackStateChangedEventArgs(PlaybackState state)
        {
            State = state;
        }
    }
}

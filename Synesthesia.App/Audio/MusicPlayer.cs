using NAudio.Wave;
using NAudio.MediaFoundation;
using System;
using System.IO;
using System.Threading.Tasks;
using Synesthesia.App.Models;

namespace Synesthesia.App.Audio
{
    public class MusicPlayer : IDisposable
    {
        private IWavePlayer? wavePlayer;
        private AudioFileReader? audioFileReader;
        private bool isDisposed;

        public event EventHandler<MusicPlayerStateChangedEventArgs>? PlaybackStateChanged;
        public event EventHandler? PlaybackStopped;

        public PlaybackState PlaybackState => wavePlayer?.PlaybackState ?? PlaybackState.Stopped;
        public TimeSpan Duration => audioFileReader?.TotalTime ?? TimeSpan.Zero;
        public TimeSpan Position
        {
            get => audioFileReader?.CurrentTime ?? TimeSpan.Zero;
            set
            {
                if (audioFileReader != null)
                {
                    audioFileReader.CurrentTime = value;
                }
            }
        }

        public float Volume
        {
            get => audioFileReader?.Volume ?? 0f;
            set
            {
                if (audioFileReader != null)
                {
                    audioFileReader.Volume = Math.Max(0f, Math.Min(1f, value));
                }
            }
        }

        static MusicPlayer()
        {
            MediaFoundationApi.Startup();
        }

        public async Task<bool> LoadTrackAsync(Track track)
        {
            try
            {
                await Task.Run(() =>
                {
                    Stop();

                    if (!File.Exists(track.FilePath))
                        return false;

                    audioFileReader = new AudioFileReader(track.FilePath);
                    wavePlayer = new WaveOutEvent();
                    wavePlayer.Init(audioFileReader);

                    wavePlayer.PlaybackStopped += OnPlaybackStopped;

                    return true;
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Play()
        {
            if (wavePlayer?.PlaybackState == PlaybackState.Paused || 
                wavePlayer?.PlaybackState == PlaybackState.Stopped)
            {
                wavePlayer.Play();
                PlaybackStateChanged?.Invoke(this, new MusicPlayerStateChangedEventArgs(PlaybackState.Playing));
            }
        }

        public void Pause()
        {
            if (wavePlayer?.PlaybackState == PlaybackState.Playing)
            {
                wavePlayer.Pause();
                PlaybackStateChanged?.Invoke(this, new MusicPlayerStateChangedEventArgs(PlaybackState.Paused));
            }
        }

        public void Stop()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                wavePlayer.PlaybackStopped -= OnPlaybackStopped;
                wavePlayer.Dispose();
                wavePlayer = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }

            PlaybackStateChanged?.Invoke(this, new MusicPlayerStateChangedEventArgs(PlaybackState.Stopped));
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            PlaybackStopped?.Invoke(this, EventArgs.Empty);
            PlaybackStateChanged?.Invoke(this, new MusicPlayerStateChangedEventArgs(PlaybackState.Stopped));
        }

        public float[] GetFFTData(int fftLength = 1024)
        {
            if (audioFileReader == null) return new float[fftLength / 2];

            // Para implementar FFT real, seria necessário adicionar processamento de áudio mais complexo
            // Por agora, vamos simular dados de FFT
            var fftData = new float[fftLength / 2];
            var random = new Random();
            
            for (int i = 0; i < fftData.Length; i++)
            {
                // Simula dados de espectro baseados na posição (frequências mais baixas têm mais energia)
                float baseAmplitude = 1.0f / (i + 1) * 0.5f;
                fftData[i] = baseAmplitude * (float)random.NextDouble();
            }

            return fftData;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                Stop();
                isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~MusicPlayer()
        {
            Dispose();
        }
    }

    public class MusicPlayerStateChangedEventArgs : EventArgs
    {
        public PlaybackState NewState { get; }

        public MusicPlayerStateChangedEventArgs(PlaybackState newState)
        {
            NewState = newState;
        }
    }
}

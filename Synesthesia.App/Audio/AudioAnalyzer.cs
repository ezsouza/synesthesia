using MathNet.Numerics.IntegralTransforms;
using NAudio.Dsp;
using System;
using Complex = System.Numerics.Complex;

namespace Synesthesia.App.Audio
{
    public class AudioAnalyzer
    {
        public int FFTSize { get; } = 1024; // tamanho da janela de amostragem (potência de 2)
        public float[] SpectrumData { get; private set; }

        private readonly Complex[] fftBuffer;
        private readonly float[] window;

        public AudioAnalyzer()
        {
            SpectrumData = new float[FFTSize / 2];
            fftBuffer = new Complex[FFTSize];
            window = GenerateHanningWindow(FFTSize);
        }

        public virtual void Analyze(float[] audioSamples)
        {
            if (audioSamples.Length < FFTSize)
                return;

            for (int i = 0; i < FFTSize; i++)
            {
                var sample = audioSamples[i] * window[i];
                fftBuffer[i] = new Complex(sample, 0.0);
            }

            Fourier.Forward(fftBuffer, FourierOptions.Matlab); // FFT de tempo para frequência

            for (int i = 0; i < SpectrumData.Length; i++)
            {
                SpectrumData[i] = (float)Math.Sqrt(
                    fftBuffer[i].Real * fftBuffer[i].Real +
                    fftBuffer[i].Imaginary * fftBuffer[i].Imaginary
                );
            }
        }

        private float[] GenerateHanningWindow(int size)
        {
            float[] window = new float[size];
            for (int i = 0; i < size; i++)
            {
                window[i] = 0.5f * (1 - (float)Math.Cos(2 * Math.PI * i / (size - 1)));
            }
            return window;
        }
    }
}

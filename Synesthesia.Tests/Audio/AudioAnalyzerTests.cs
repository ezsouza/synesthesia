using Synesthesia.App.Audio;
using Xunit;
using System;
using System.Linq;

namespace Synesthesia.Tests.Audio
{
    public class AudioAnalyzerTests
    {
        [Fact]
        public void Analyzer_Should_Have_Default_FFT_Size()
        {
            var analyzer = new AudioAnalyzer();
            Assert.Equal(1024, analyzer.FFTSize);
            Assert.Equal(512, analyzer.SpectrumData.Length); // FFTSize / 2
        }

        [Fact]
        public void Analyze_Should_Not_Throw_When_Sample_Size_Is_Insufficient()
        {
            var analyzer = new AudioAnalyzer();
            var smallBuffer = new float[128]; // menor que 1024

            Exception? ex = Record.Exception(() => analyzer.Analyze(smallBuffer));
            Assert.Null(ex);
        }

        [Fact]
        public void Analyze_Should_Populate_SpectrumData_With_Positive_Values()
        {
            var analyzer = new AudioAnalyzer();
            var buffer = new float[analyzer.FFTSize];

            // Preencher buffer com um sinal de onda simples (ex: seno em baixa frequência)
            double freq = 440.0; // A4 (Hz)
            double sampleRate = 44100.0;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (float)Math.Sin(2 * Math.PI * freq * i / sampleRate);
            }

            analyzer.Analyze(buffer);

            Assert.All(analyzer.SpectrumData, value => Assert.True(value >= 0));
            Assert.Contains(analyzer.SpectrumData, val => val > 0); // deve haver energia no espectro
        }

        [Fact]
        public void SpectrumData_Should_Reflect_Strong_Low_Frequency()
        {
            var analyzer = new AudioAnalyzer();
            var buffer = new float[analyzer.FFTSize];

            // Sinal contínuo de frequência muito baixa
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (float)Math.Sin(2 * Math.PI * 60 * i / 44100.0); // 60Hz
            }

            analyzer.Analyze(buffer);

            int strongestBand = analyzer.SpectrumData
                .Select((value, index) => new { value, index })
                .OrderByDescending(x => x.value)
                .First().index;

            // Espera-se que a energia esteja nos primeiros índices do espectro (baixas frequências)
            Assert.InRange(strongestBand, 0, analyzer.SpectrumData.Length / 4);
        }
    }
}

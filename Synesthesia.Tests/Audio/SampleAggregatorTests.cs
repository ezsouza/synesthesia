using Xunit;
using Synesthesia.App.Audio;
using NAudio.Wave;
using System;
using System.Linq;

namespace Synesthesia.Tests.Audio
{
    public class SampleAggregatorTests
    {
        private class MockSampleProvider : ISampleProvider
        {
            private readonly float[] data;
            private int position = 0;

            public MockSampleProvider(float[] data)
            {
                this.data = data;
                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            }

            public WaveFormat WaveFormat { get; }

            public int Read(float[] buffer, int offset, int count)
            {
                int samplesToCopy = Math.Min(count, data.Length - position);
                Array.Copy(data, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;
                return samplesToCopy;
            }
        }

        private class TestAnalyzer : AudioAnalyzer
        {
            public int AnalyzeCallCount { get; private set; } = 0;

            public override void Analyze(float[] buffer)
            {
                base.Analyze(buffer);
                AnalyzeCallCount++;
            }
        }

        [Fact]
        public void SampleAggregator_Should_Pass_Data_To_Analyzer()
        {
            // Arrange
            int fftSize = 1024;
            float[] audioData = Enumerable.Repeat(0.5f, fftSize * 2).ToArray(); // 2 blocos
            var mockProvider = new MockSampleProvider(audioData);
            var analyzer = new TestAnalyzer();

            var aggregator = new SampleAggregator(mockProvider, analyzer, fftSize);

            float[] output = new float[audioData.Length];

            // Act
            int read = aggregator.Read(output, 0, output.Length);

            // Assert
            Assert.Equal(audioData.Length, read); // deve ler tudo
            Assert.Equal(2, analyzer.AnalyzeCallCount); // dois blocos FFTSize
        }

        [Fact]
        public void SampleAggregator_Should_Handle_Partial_Blocks_Gracefully()
        {
            // Arrange
            int fftSize = 1024;
            float[] audioData = Enumerable.Repeat(0.5f, fftSize + 100).ToArray(); // um bloco e um resto
            var mockProvider = new MockSampleProvider(audioData);
            var analyzer = new TestAnalyzer();

            var aggregator = new SampleAggregator(mockProvider, analyzer, fftSize);

            float[] output = new float[audioData.Length];

            // Act
            int read = aggregator.Read(output, 0, output.Length);

            // Assert
            Assert.Equal(audioData.Length, read); // leitura total
            Assert.Equal(1, analyzer.AnalyzeCallCount); // apenas 1 bloco analisado
        }

        [Fact]
        public void SampleAggregator_Should_Keep_WaveFormat()
        {
            // Arrange
            var mockProvider = new MockSampleProvider(new float[512]);
            var analyzer = new TestAnalyzer();
            var aggregator = new SampleAggregator(mockProvider, analyzer);

            // Assert
            Assert.NotNull(aggregator.WaveFormat);
            Assert.Equal(44100, aggregator.WaveFormat.SampleRate);
            Assert.Equal(1, aggregator.WaveFormat.Channels);
        }
    }
}

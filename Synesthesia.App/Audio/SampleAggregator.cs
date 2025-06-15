using NAudio.Wave;
using Synesthesia.App.Audio;

namespace Synesthesia.App.Audio
{
    public class SampleAggregator : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly AudioAnalyzer analyzer;
        private readonly float[] buffer;

        public SampleAggregator(ISampleProvider source, AudioAnalyzer analyzer, int fftSize = 1024)
        {
            this.source = source;
            this.analyzer = analyzer;
            this.buffer = new float[fftSize];
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] outputBuffer, int offset, int count)
        {
            int samplesRead = source.Read(outputBuffer, offset, count);

            int remaining = count;
            int bufferIndex = 0;

            // Captura apenas um bloco FFTSize por vez
            while (remaining >= buffer.Length)
            {
                Array.Copy(outputBuffer, offset + bufferIndex, buffer, 0, buffer.Length);
                analyzer.Analyze(buffer);
                bufferIndex += buffer.Length;
                remaining -= buffer.Length;
            }

            return samplesRead;
        }
    }
}

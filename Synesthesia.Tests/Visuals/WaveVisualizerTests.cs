using Synesthesia.App.Visuals;
using System.Windows.Controls;
using System.Threading;
using Xunit;

namespace Synesthesia.Tests.Visuals
{
    public class WaveVisualizerTests
    {
        [Fact]
        public void Render_ShouldNotThrow_WithValidData()
        {
            Thread staThread = new Thread(() =>
            {
                var canvas = new Canvas { Width = 800, Height = 400 };
                var visualizer = new WaveVisualizer(canvas);

                var spectrum = new float[64];
                for (int i = 0; i < spectrum.Length; i++)
                    spectrum[i] = (float)(0.5f * System.Math.Sin(i));

                var ex = Xunit.Record.Exception(() => visualizer.Render(spectrum));
                Assert.Null(ex);
            });
            
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}

using Synesthesia.App.Visuals;
using System.Windows.Controls;
using System.Threading;
using Xunit;

namespace Synesthesia.Tests.Visuals
{
    public class BarsVisualizerTests
    {
        [Fact]
        public void Render_ShouldNotThrow_WithValidSpectrum()
        {
            Thread staThread = new Thread(() =>
            {
                var canvas = new Canvas { Width = 800, Height = 400 };
                var visualizer = new BarsVisualizer(canvas);

                var spectrum = new float[64];
                for (int i = 0; i < spectrum.Length; i++)
                    spectrum[i] = 0.5f;

                var ex = Xunit.Record.Exception(() => visualizer.Render(spectrum));
                Assert.Null(ex);
            });
            
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}

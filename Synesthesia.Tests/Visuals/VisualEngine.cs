using Synesthesia.App.Visuals;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Threading;
using Xunit;

namespace Synesthesia.Tests.Visuals
{
    public class VisualEngineTests
    {
        [Fact]
        public void Render_ShouldDrawExpectedNumberOfBars()
        {
            Thread staThread = new Thread(() =>
            {
                // Arrange
                var canvas = new Canvas
                {
                    Width = 800,
                    Height = 400
                };

                var engine = new VisualEngine(canvas);

                float[] spectrum = new float[64];
                for (int i = 0; i < spectrum.Length; i++)
                    spectrum[i] = 0.5f;

                // Act
                engine.Render(spectrum);

                // Assert
                Assert.Equal(64, canvas.Children.Count);
                foreach (var child in canvas.Children)
                {
                    Assert.IsType<Rectangle>(child);
                }
            });
            
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }

        [Fact]
        public void Render_ShouldClearPreviousElements()
        {
            Thread staThread = new Thread(() =>
            {
                // Arrange
                var canvas = new Canvas
                {
                    Width = 800,
                    Height = 400
                };
                var engine = new VisualEngine(canvas);

                canvas.Children.Add(new Rectangle()); // conteúdo prévio
                Assert.Single(canvas.Children);

                // Act
                engine.Render(new float[32]);

                // Assert
                Assert.Equal(32, canvas.Children.Count);
            });
            
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }

        [Fact]
        public void Render_ShouldNotThrow_OnInvalidConditions()
        {
            Thread staThread = new Thread(() =>
            {
                var canvas = new Canvas { Width = 0, Height = 0 };
                var engine = new VisualEngine(canvas);

                // Não deve lançar exceção se o canvas não estiver pronto ou espectro for nulo
                var ex1 = Xunit.Record.Exception(() => engine.Render(null!));
                var ex2 = Xunit.Record.Exception(() => engine.Render(new float[0]));

                Assert.Null(ex1);
                Assert.Null(ex2);
            });
            
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }

        [Fact]
        public void Clear_ShouldRemoveAllVisuals()
        {
            Thread staThread = new Thread(() =>
            {
                // Arrange
                var canvas = new Canvas();
                canvas.Children.Add(new Rectangle());
                canvas.Children.Add(new Rectangle());

                var engine = new VisualEngine(canvas);

                // Act
                engine.Clear();

                // Assert
                Assert.Empty(canvas.Children);
            });
            
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}

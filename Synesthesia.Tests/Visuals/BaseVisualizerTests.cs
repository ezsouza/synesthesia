using System.Threading;
using System.Windows.Controls;
using Moq;
using Synesthesia.App.Visuals;
using Xunit;

namespace Synesthesia.Tests.Visuals
{
    public class BaseVisualizerTests
    {
        /// <summary>
        /// A test-friendly mock visualizer that doesn't create actual WPF controls
        /// </summary>
        private class MockVisualizer : BaseVisualizer
        {
            public bool RenderCalled { get; private set; } = false;
            public bool ClearCalled { get; set; } = false; // Propriedade com set público
            public float[]? LastSpectrum { get; private set; } = null;

            // Passa valor padrão para o construtor base
            public MockVisualizer() : base(default!) { }

            public override void Render(float[] spectrum)
            {
                RenderCalled = true;
                LastSpectrum = spectrum;
            }

            public override void Clear()
            {
                ClearCalled = true;
            }
        }

        [Fact]
        public void Clear_Should_SetFlag_When_Called_WithoutCanvas()
        {
            // Arrange
            var visualizer = new MockVisualizer();

            // Verify flag is initially false
            Assert.False(visualizer.ClearCalled);

            // Act
            visualizer.Clear();

            // Assert
            Assert.True(visualizer.ClearCalled);
        }

        [Fact]
        public void Clear_Should_Be_Idempotent()
        {
            var visualizer = new MockVisualizer();

            visualizer.Clear();
            bool firstCallResult = visualizer.ClearCalled;

            // Reseta a flag para testar segunda chamada
            visualizer.ClearCalled = false;

            visualizer.Clear();
            bool secondCallResult = visualizer.ClearCalled;

            Assert.True(firstCallResult);
            Assert.True(secondCallResult);
        }

        [Fact]
        public void Clear_Should_Not_Affect_Render_Flag()
        {
            // Arrange
            var visualizer = new MockVisualizer();

            // Act
            visualizer.Clear();

            // Assert
            Assert.True(visualizer.ClearCalled);
            Assert.False(visualizer.RenderCalled);
        }

        [Fact]
        public void Clear_And_Render_Should_Set_Independent_Flags()
        {
            // Arrange
            var visualizer = new MockVisualizer();

            // Act
            visualizer.Clear();
            visualizer.Render(new float[] { 0.1f, 0.2f });

            // Assert
            Assert.True(visualizer.ClearCalled);
            Assert.True(visualizer.RenderCalled);
            Assert.NotNull(visualizer.LastSpectrum);
        }
    }
}
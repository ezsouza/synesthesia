using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Synesthesia.App.Visuals
{
    public class VisualEngine
    {
        private readonly Canvas canvas;
        private readonly Dictionary<string, BaseVisualizer> visualizers;
        private BaseVisualizer currentVisualizer;

        public VisualEngine(Canvas targetCanvas)
        {
            canvas = targetCanvas;

            visualizers = new Dictionary<string, BaseVisualizer>
            {
                { "Bars", new BarsVisualizer(canvas) },
                { "Wave", new WaveVisualizer(canvas) },
                { "Kaleido", new KaleidoVisualizer(canvas) }
            };

            currentVisualizer = visualizers["Bars"];
        }

        public void SetVisualizer(string name)
        {
            if (visualizers.TryGetValue(name, out var visualizer))
                currentVisualizer = visualizer;
        }

        public void Render(float[] spectrum)
        {
            currentVisualizer?.Render(spectrum);
        }

        public void Clear()
        {
            currentVisualizer?.Clear();
        }
    }
}

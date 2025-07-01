using System.Collections.Generic;
using System.Windows.Controls;

namespace Synesthesia.App.Visuals
{
    public class VisualEngine
    {
        private readonly Canvas canvas;
        private readonly Dictionary<string, BaseVisualizer> visualizers = new();
        private BaseVisualizer? currentVisualizer;

        public VisualEngine(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void AddVisualizer(string name, BaseVisualizer visualizer)
        {
            visualizers[name] = visualizer;
            currentVisualizer ??= visualizer;
        }

        public void SetVisualizer(string name)
        {
            if (visualizers.TryGetValue(name, out var visualizer))
            {
                currentVisualizer?.Clear();
                currentVisualizer = visualizer;
            }
        }

        public void Render(float[] spectrum)
        {
            if (spectrum == null || spectrum.Length == 0) return;
            currentVisualizer?.Render(spectrum);
        }

        public void Clear()
        {
            currentVisualizer?.Clear();
        }

        public IEnumerable<string> GetAvailableVisualizers()
        {
            return visualizers.Keys;
        }

        public string? GetCurrentVisualizerName()
        {
            foreach (var kvp in visualizers)
            {
                if (kvp.Value == currentVisualizer)
                    return kvp.Key;
            }
            return null;
        }
    }
}

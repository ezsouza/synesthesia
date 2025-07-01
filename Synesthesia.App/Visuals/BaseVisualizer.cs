using System.Windows.Controls;

namespace Synesthesia.App.Visuals
{
    public abstract class BaseVisualizer
    {
        protected readonly Canvas canvas;

        protected BaseVisualizer(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public abstract void Render(float[] spectrum);
        public abstract void Clear();
    }
}

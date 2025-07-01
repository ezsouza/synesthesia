using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Synesthesia.App.Visuals
{
    public class WaveVisualizer : BaseVisualizer
    {
        public WaveVisualizer(Canvas canvas) : base(canvas) { }

        public override void Render(float[] spectrum)
        {
            if (spectrum == null || spectrum.Length == 0) return;
            
            canvas.Children.Clear();
            var polyline = new Polyline
            {
                Stroke = Brushes.Cyan,
                StrokeThickness = 2
            };

            double width = canvas.ActualWidth > 0 ? canvas.ActualWidth : canvas.Width;
            double height = canvas.ActualHeight > 0 ? canvas.ActualHeight : canvas.Height;
            
            if (width <= 0 || height <= 0) return;

            for (int i = 0; i < spectrum.Length; i++)
            {
                double x = i * (width / spectrum.Length);
                double y = height / 2 - (spectrum[i] * height / 2);
                polyline.Points.Add(new System.Windows.Point(x, y));
            }

            canvas.Children.Add(polyline);
        }

        public override void Clear() => canvas.Children.Clear();
    }
}

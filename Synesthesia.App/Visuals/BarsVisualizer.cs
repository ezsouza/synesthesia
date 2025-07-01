using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Synesthesia.App.Visuals
{
    public class BarsVisualizer : BaseVisualizer
    {
        public BarsVisualizer(Canvas canvas) : base(canvas) { }

        public override void Render(float[] spectrum)
        {
            if (spectrum == null || spectrum.Length == 0) return;
            
            canvas.Children.Clear();
            int barCount = spectrum.Length;
            double canvasWidth = canvas.ActualWidth > 0 ? canvas.ActualWidth : canvas.Width;
            double canvasHeight = canvas.ActualHeight > 0 ? canvas.ActualHeight : canvas.Height;
            
            if (canvasWidth <= 0 || canvasHeight <= 0) return;
            
            double barWidth = canvasWidth / barCount;

            for (int i = 0; i < barCount; i++)
            {
                double magnitude = Math.Clamp(spectrum[i] * 10, 2, Math.Max(2, canvasHeight));
                var bar = new Rectangle
                {
                    Width = barWidth - 1,
                    Height = magnitude,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)(i * 255 / barCount), 255, 100))
                };

                Canvas.SetLeft(bar, i * barWidth);
                Canvas.SetTop(bar, canvasHeight - magnitude);
                canvas.Children.Add(bar);
            }
        }

        public override void Clear() => canvas.Children.Clear();
    }
}

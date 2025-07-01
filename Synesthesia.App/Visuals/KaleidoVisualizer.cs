using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Synesthesia.App.Visuals
{
    public class KaleidoVisualizer : BaseVisualizer
    {
        private readonly Random rand = new();

        public KaleidoVisualizer(Canvas canvas) : base(canvas) { }

        public override void Render(float[] spectrum)
        {
            if (spectrum == null || spectrum.Length == 0) return;
            
            canvas.Children.Clear();

            double centerX = canvas.ActualWidth > 0 ? canvas.ActualWidth / 2 : canvas.Width / 2;
            double centerY = canvas.ActualHeight > 0 ? canvas.ActualHeight / 2 : canvas.Height / 2;

            if (centerX <= 0 || centerY <= 0) return;

            int points = 60;

            for (int i = 0; i < points; i++)
            {
                double angle = 2 * Math.PI * i / points;
                double radius = spectrum[i % spectrum.Length] * 150;

                double x = centerX + Math.Cos(angle) * radius;
                double y = centerY + Math.Sin(angle) * radius;

                var ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), 255))
                };

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                canvas.Children.Add(ellipse);
            }
        }

        public override void Clear() => canvas.Children.Clear();
    }
}

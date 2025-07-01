using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Synesthesia.App.Visuals
{
    public class SpectrumVisualizer : BaseVisualizer
    {
        private readonly Random rand = new();

        public SpectrumVisualizer(Canvas canvas) : base(canvas) { }

        public override void Render(float[] spectrum)
        {
            if (spectrum == null || spectrum.Length == 0) return;
            
            canvas.Children.Clear();

            double centerX = canvas.ActualWidth > 0 ? canvas.ActualWidth / 2 : canvas.Width / 2;
            double centerY = canvas.ActualHeight > 0 ? canvas.ActualHeight / 2 : canvas.Height / 2;
            
            if (centerX <= 0 || centerY <= 0) return;

            double radius = Math.Min(centerX, centerY) * 0.8;
            int bands = Math.Min(spectrum.Length, 64);

            for (int i = 0; i < bands; i++)
            {
                double angle = (2 * Math.PI * i) / bands;
                double magnitude = spectrum[i] * radius * 0.5;
                
                // Linha principal do espectro
                var line = new Line
                {
                    X1 = centerX + Math.Cos(angle) * (radius - magnitude),
                    Y1 = centerY + Math.Sin(angle) * (radius - magnitude),
                    X2 = centerX + Math.Cos(angle) * radius,
                    Y2 = centerY + Math.Sin(angle) * radius,
                    Stroke = new SolidColorBrush(Color.FromRgb(
                        (byte)(255 - i * 255 / bands),
                        (byte)(i * 255 / bands),
                        (byte)(255 * spectrum[i]))),
                    StrokeThickness = 2 + magnitude * 0.1
                };

                canvas.Children.Add(line);

                // Efeito de brilho
                if (spectrum[i] > 0.3f)
                {
                    var glow = new Ellipse
                    {
                        Width = magnitude * 0.5,
                        Height = magnitude * 0.5,
                        Fill = new RadialGradientBrush(
                            Color.FromArgb(100, 255, 255, 255),
                            Color.FromArgb(0, 255, 255, 255))
                    };

                    Canvas.SetLeft(glow, centerX + Math.Cos(angle) * radius - glow.Width / 2);
                    Canvas.SetTop(glow, centerY + Math.Sin(angle) * radius - glow.Height / 2);
                    canvas.Children.Add(glow);
                }
            }
        }

        public override void Clear() => canvas.Children.Clear();
    }
}

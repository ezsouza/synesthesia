using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Synesthesia.App.Visuals
{
    public class ParticleFlowVisualizer : BaseVisualizer
    {
        private readonly List<Particle> particles = new();
        private readonly Random rand = new();
        private int frameCount = 0;

        public ParticleFlowVisualizer(Canvas canvas) : base(canvas) { }

        public override void Render(float[] spectrum)
        {
            if (spectrum == null || spectrum.Length == 0) return;
            
            canvas.Children.Clear();

            double centerX = canvas.ActualWidth > 0 ? canvas.ActualWidth / 2 : canvas.Width / 2;
            double centerY = canvas.ActualHeight > 0 ? canvas.ActualHeight / 2 : canvas.Height / 2;
            
            if (centerX <= 0 || centerY <= 0) return;

            frameCount++;

            // Adicionar novas partículas baseadas no espectro
            for (int i = 0; i < spectrum.Length && i < 32; i++)
            {
                if (spectrum[i] > 0.1f && rand.NextDouble() < spectrum[i])
                {
                    var particle = new Particle
                    {
                        X = centerX + rand.NextDouble() * 100 - 50,
                        Y = centerY + rand.NextDouble() * 100 - 50,
                        VelocityX = (rand.NextDouble() - 0.5) * spectrum[i] * 10,
                        VelocityY = (rand.NextDouble() - 0.5) * spectrum[i] * 10,
                        Life = 1.0f,
                        Size = spectrum[i] * 20 + 5,
                        Color = Color.FromRgb(
                            (byte)(255 * spectrum[i]),
                            (byte)(255 - i * 255 / spectrum.Length),
                            (byte)(255 * (1 - spectrum[i])))
                    };
                    particles.Add(particle);
                }
            }

            // Atualizar e renderizar partículas
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var particle = particles[i];
                
                // Atualizar posição
                particle.X += particle.VelocityX;
                particle.Y += particle.VelocityY;
                
                // Efeito de gravidade/atração ao centro
                double dx = centerX - particle.X;
                double dy = centerY - particle.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                
                if (distance > 0)
                {
                    particle.VelocityX += dx * 0.001;
                    particle.VelocityY += dy * 0.001;
                }

                // Diminuir vida
                particle.Life -= 0.02f;

                if (particle.Life <= 0)
                {
                    particles.RemoveAt(i);
                    continue;
                }

                // Renderizar partícula
                var ellipse = new Ellipse
                {
                    Width = particle.Size * particle.Life,
                    Height = particle.Size * particle.Life,
                    Fill = new RadialGradientBrush(
                        Color.FromArgb((byte)(255 * particle.Life), particle.Color.R, particle.Color.G, particle.Color.B),
                        Color.FromArgb(0, particle.Color.R, particle.Color.G, particle.Color.B))
                };

                Canvas.SetLeft(ellipse, particle.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, particle.Y - ellipse.Height / 2);
                canvas.Children.Add(ellipse);

                // Adicionar trilha se a partícula está se movendo rápido
                double speed = Math.Sqrt(particle.VelocityX * particle.VelocityX + particle.VelocityY * particle.VelocityY);
                if (speed > 2)
                {
                    var trail = new Line
                    {
                        X1 = particle.X,
                        Y1 = particle.Y,
                        X2 = particle.X - particle.VelocityX * 3,
                        Y2 = particle.Y - particle.VelocityY * 3,
                        Stroke = new SolidColorBrush(Color.FromArgb(
                            (byte)(100 * particle.Life),
                            particle.Color.R, particle.Color.G, particle.Color.B)),
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(trail);
                }
            }

            // Limitar número de partículas para performance
            while (particles.Count > 200)
            {
                particles.RemoveAt(0);
            }
        }

        public override void Clear()
        {
            canvas.Children.Clear();
            particles.Clear();
        }

        private class Particle
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double VelocityX { get; set; }
            public double VelocityY { get; set; }
            public float Life { get; set; }
            public double Size { get; set; }
            public Color Color { get; set; }
        }
    }
}

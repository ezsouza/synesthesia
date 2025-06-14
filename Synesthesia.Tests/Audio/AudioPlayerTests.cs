using Synesthesia.App.Audio;
using Xunit;
using System;
using System.IO;

namespace Synesthesia.Tests
{
    public class AudioPlayerTests
    {
        [Fact]
        public void AudioPlayer_Should_Initialize_Correctly()
        {
            // Arrange & Act
            using var player = new AudioPlayer();
            
            // Assert
            Assert.False(player.IsPlaying);
            Assert.False(player.IsPaused);
            Assert.False(player.IsLoaded);
            Assert.Equal(TimeSpan.Zero, player.CurrentTime);
            Assert.Equal(TimeSpan.Zero, player.TotalTime);
            Assert.Equal(0f, player.Volume);
        }

        [Fact]
        public void AudioPlayer_Should_Throw_When_File_Not_Found()
        {
            // Arrange
            using var player = new AudioPlayer();
            
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => player.Load("arquivo_inexistente.mp3"));
        }

        [Fact]
        public void AudioPlayer_Should_Throw_When_Format_Not_Supported()
        {
            // Arrange
            using var player = new AudioPlayer();
            
            // Criar um arquivo temporário com extensão inválida
            var tempFile = Path.GetTempFileName();
            var invalidFile = Path.ChangeExtension(tempFile, ".txt");
            File.Move(tempFile, invalidFile);
            
            try
            {
                // Act & Assert
                Assert.Throws<NotSupportedException>(() => player.Load(invalidFile));
            }
            finally
            {
                // Cleanup - limpar o arquivo temporário
                if (File.Exists(invalidFile))
                    File.Delete(invalidFile);
            }
        }

        [Fact]
        public void AudioPlayer_Should_Throw_When_Play_Without_Load()
        {
            // Arrange
            using var player = new AudioPlayer();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => player.Play());
        }

        [Fact]
        public void AudioPlayer_Volume_Should_Be_Clamped_To_Valid_Range()
        {
            // Arrange
            using var player = new AudioPlayer();
            
            // Act & Assert - Volume acima do máximo
            player.Volume = 2.0f;
            Assert.True(player.Volume <= 1.0f);
            
            // Act & Assert - Volume abaixo do mínimo
            player.Volume = -0.5f;
            Assert.True(player.Volume >= 0.0f);
        }
    }
}
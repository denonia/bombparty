using System.IO;
using System.Media;

namespace BombParty.Services
{
    public class AudioService : IAudioService
    {
        private readonly SoundPlayer gameOverPlayer = new SoundPlayer();

        public AudioService()
        {
            var gameOverStream = new MemoryStream(Properties.Resources.winner);
            gameOverPlayer = new SoundPlayer(gameOverStream);
        }

        public void PlayGameOver()
        {
            gameOverPlayer.Play();
        }

        public void Stop()
        {
            gameOverPlayer.Stop();
        }
    }
}

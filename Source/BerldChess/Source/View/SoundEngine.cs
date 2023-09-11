using BerldChess.Properties;
using System;
using System.Media;
using System.Threading.Tasks;

namespace BerldChess.View
{
    internal class SoundEngine
    {
        private const int TimeBetweenSoundsMilliseconds = 100;

        private readonly SoundPlayer[] _players;

        private readonly SoundPlayer _movePlayer = new SoundPlayer(Resources.Move);
        private readonly SoundPlayer _castlingPlayer = new SoundPlayer(Resources.Castling);
        private readonly SoundPlayer _capturePlayer = new SoundPlayer(Resources.Capture);
        private readonly SoundPlayer _illegalPlayer = new SoundPlayer(Resources.Ilegal);

        private DateTime _lastPlayed = DateTime.MinValue;
        private SoundPlayer _toPlay = null;


        public SoundEngine()
        {
            _players = new SoundPlayer[]
            {
                _movePlayer,
                _castlingPlayer,
                _capturePlayer,
                _illegalPlayer
            };
        }

        internal void Load()
        {
            foreach (SoundPlayer player in _players)
            {
                player.LoadAsync();
            }
        }

        internal void PlayMove()
        {
            PlayWithTimeBetween(_movePlayer);
        }

        internal void PlayCastling()
        {
            PlayWithTimeBetween(_castlingPlayer);
        }

        internal void PlayCapture()
        {
            PlayWithTimeBetween(_capturePlayer);
        }

        internal void PlayIllegal()
        {
            PlayWithTimeBetween(_illegalPlayer);
        }

        private void PlayWithTimeBetween(SoundPlayer player)
        {
            if (!player.IsLoadCompleted)
                return;

            TimeSpan timeSinceLastPlayed = DateTime.Now - _lastPlayed;

            if (timeSinceLastPlayed.TotalMilliseconds < TimeBetweenSoundsMilliseconds)
            {
                if (_toPlay == null)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeBetweenSoundsMilliseconds - (int)timeSinceLastPlayed.TotalMilliseconds);
                        Play(player);
                    });
                }

                _toPlay = player;
            }
            else
            {
                Play(player);
            }
        }

        private void Play(SoundPlayer player)
        {
            player.Play();
            _lastPlayed = DateTime.Now;
            _toPlay = null;
        }
    }
}

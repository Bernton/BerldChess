using System;

namespace BerldChess.Model
{
    [Serializable]
    public class Level
    {
        public LevelType SelectedLevelType { get; set; }
        public int Plies { get; set; }
        public long TimePerMove { get; set; }
        public long TotalTime { get; set; }
        public long Increment { get; set; }
        public long Nodes { get; set; }

        public Level()
        {
            SelectedLevelType = LevelType.TimePerMove;
            Plies = 16;
            TimePerMove = 1000;
            TotalTime = 180000;
            Increment = 2000;
            Nodes = 1000000;
        }
    }
}

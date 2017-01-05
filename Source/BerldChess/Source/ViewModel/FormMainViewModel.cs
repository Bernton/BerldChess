using BerldChess.Model;
using ChessDotNet;
using ChessEngineInterface;
using System.Collections.Generic;
using System.IO;

namespace BerldChess.ViewModel
{
    public class FormMainViewModel
    {
        #region Fields

        public const int MultiPV = 200;
        public const string EngineArgsFN = "engineArgs.txt";
        public const string ConfigFileName = "config.txt";

        #endregion

        #region Properties

        public int NavIndex { get; set; }
        public Engine Engine { get; set; }
        public ChessGame Game { get; set; }
        public List<ChessPosition> PositionHistory { get; set; }

        #endregion

        #region Constructors

        public FormMainViewModel()
        {
            string[] engineArgs = null;



            NavIndex = 0;
            PositionHistory = new List<ChessPosition>();
            Engine = new Engine("engine.exe");
            Engine.Query($"setoption name MultiPV value {MultiPV}");

            if (File.Exists(EngineArgsFN))
            {
                engineArgs = File.ReadAllLines(EngineArgsFN);

                for (int i = 0; i < engineArgs.Length; i++)
                {
                    Engine.Query(engineArgs[i]);
                }
            }

            Engine.Query("ucinewgame");
            Engine.Query("go infinite");
            Game = new ChessGame();
        }

        #endregion
    }
}

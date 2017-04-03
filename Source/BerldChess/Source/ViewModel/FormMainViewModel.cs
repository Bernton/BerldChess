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

        public const string EngineArgumentsFileName = "engineArgs.txt";
        public const string ConfigurationFileName = "config.txt";

        #endregion

        #region Properties

        public int NavigationIndex { get; set; }
        public Engine Engine { get; set; }
        public ChessGame Game { get; set; }
        public List<ChessPosition> PositionHistory { get; set; }

        #endregion

        #region Constructors

        public FormMainViewModel()
        {
            NavigationIndex = 0;
            PositionHistory = new List<ChessPosition>();
            Engine = new Engine("engine.exe");

            string[] engineArgs = null;

            if (File.Exists(EngineArgumentsFileName))
            {
                engineArgs = File.ReadAllLines(EngineArgumentsFileName);

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

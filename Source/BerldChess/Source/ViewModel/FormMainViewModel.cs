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

        public const string ConfigurationFileName = "config.txt";

        #endregion

        #region Properties

        public int NavigationIndex { get; set; }
        public Engine Engine { get; set; }
        public ChessGame Game { get; set; }
        public List<ChessPly> PlyList { get; set; }

        public ChessPly LatestPly
        {
            get
            {
                if(PlyList.Count > 0)
                {
                    return PlyList[PlyList.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public ChessPly CurrentPly
        {
            get
            {
                if(PlyList.Count > 0)
                {
                    return PlyList[NavigationIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Constructors

        public FormMainViewModel()
        {
            NavigationIndex = 0;
            PlyList = new List<ChessPly>();

            Game = new ChessGame();
            PlyList.Add(new ChessPly(Game.GetFen()));
        }

        #endregion
    }
}

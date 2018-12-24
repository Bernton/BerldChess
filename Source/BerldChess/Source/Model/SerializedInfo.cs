using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace BerldChess.Model
{
    [Serializable]
    public class SerializedInfo
    {
        public static SerializedInfo Instance { get; set; } = new SerializedInfo();

        public bool UseImages { get; set; } = false;
        public bool IvoryMode { get; set; } = false;
        public bool PgnAnalysis { get; set; } = false;
        public bool HideArrows { get; set; }
        public bool BoardFlipped { get; set; }
        public bool DisplayCoordinates { get; set; } = false;
        public bool DisplayLegalMoves { get; set; } = true;
        public bool IllegalSound { get; set; } = true;
        public bool Gradient { get; set; } = true;
        public bool DarkMode { get; set; } = false;
        public bool BorderHighlight { get; set; } = false;
        public bool AutoCheck { get; set; } = false;
        public bool LocalMode { get; set; } = false;
        public bool HideOutput { get; set; } = true;
        public bool DisplayGridBorder { get; set; }
        public bool IsMaximized { get; set; } = true;
        public bool Sound { get; set; } = true;
        public bool CheatMode { get; set; }
        public int PgnAnalysisDepth { get; set; }
        public int MultiPv { get; set; } = 250;
        public int ClickDelay { get; set; } = 120;
        public int SelectedFontIndex { get; set; } = 0;
        public int AnimationTime { get; set; } = 300;
        public int? SplitterDistance { get; set; } = null;
        public double PieceSizeFactor { get; set; } = 1;
        public string LightSquarePath { get; set; }
        public string DarkSquarePath { get; set; }
        public string LastPgnDir { get; set; } = "";
        public EngineMode EngineMode { get; set; } = EngineMode.Disabled;
        public Level Level { get; set; } = new Level();
        public Rectangle? Bounds { get; set; } = null;
        public EngineList EngineList { get; set; } = new EngineList();
        public ChessFont SelectedChessFont => ChessFonts[SelectedFontIndex];
        public List<ChessFont> ChessFonts { get; set; } = new List<ChessFont>();

        [XmlElement("EngineDarkSquare")]
        public int EngineDarkSquareAsArgb
        {
            get { return EngineDarkSquare.ToArgb(); }
            set { EngineDarkSquare = Color.FromArgb(value); }
        }

        [XmlElement("EngineLightSquare")]
        public int EngineLightSquareAsArgb
        {
            get { return EngineLightSquare.ToArgb(); }
            set { EngineLightSquare = Color.FromArgb(value); }
        }

        [XmlElement("BoardDarkSquare")]
        public int BoardDarkSquareAsArgb
        {
            get { return BoardDarkSquare.ToArgb(); }
            set { BoardDarkSquare = Color.FromArgb(value); }
        }

        [XmlElement("BoardLightSquare")]
        public int BoardLightSquareAsArgb
        {
            get { return BoardLightSquare.ToArgb(); }
            set { BoardLightSquare = Color.FromArgb(value); }
        }

        [XmlIgnore] public Color BoardDarkSquare { get; set; } = Color.FromArgb(140, 162, 173);
        [XmlIgnore] public Color BoardLightSquare { get; set; } = Color.FromArgb(222, 227, 230);
        [XmlIgnore] public Color EngineDarkSquare { get; set; } = Color.FromArgb(186, 85, 70);
        [XmlIgnore] public Color EngineLightSquare { get; set; } = Color.FromArgb(240, 216, 191);

        private SerializedInfo() { }
    }
}
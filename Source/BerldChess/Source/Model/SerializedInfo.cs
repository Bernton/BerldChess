using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace BerldChess.Model
{
    [Serializable]
    public class SerializedInfo
    {
        #region Singleton

        private static SerializedInfo _instance = new SerializedInfo();

        public static SerializedInfo Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region Fields

        private bool _pgnAnalysis = false;
        private bool _hideArrows;
        private bool _boardFlipped;
        private bool _cheatMode;
        private bool _displayGridBorder;
        private bool _sound = true;
        private bool _gradient = true;
        private bool _hideOutput = true;
        private bool _isMaximized = true;
        private bool _illegalSound = true;
        private bool _displayLegalMoves = true;
        private bool _darkMode = false;
        private bool _autoCheck = false;
        private bool _localMode = false;
        private bool _borderHighlight = false;
        private bool _displayCoordinates = false;
        private int _selectedFontIndex = 0;
        private int _clickDelay = 120;
        private int _multiPV = 250;
        private int _animTime = 300;
        private int _pgnAnalysisDepth;
        private int? _splitterDistance = null;
        private double _sizeFactor = 1;
        private string _lastPgnDir = "";
        private EngineMode _engineMode = EngineMode.Disabled;
        private Rectangle? _bounds = null;
        private EngineList _engineList = new EngineList();
        private List<ChessFont> _chessFonts = new List<ChessFont>();
        private Level _level = new Level();

        #endregion

        #region Properties

        public bool PgnAnalysis
        {
            get
            {
                return _pgnAnalysis;
            }
            set
            {
                _pgnAnalysis = value;
            }
        }

        public bool HideArrows
        {
            get
            {
                return _hideArrows;
            }
            set
            {
                _hideArrows = value;
            }
        }

        public bool BoardFlipped
        {
            get
            {
                return _boardFlipped;
            }
            set
            {
                _boardFlipped = value;
            }
        }

        public bool DisplayCoordinates
        {
            get
            {
                return _displayCoordinates;
            }

            set
            {
                _displayCoordinates = value;
            }
        }

        public bool DisplayLegalMoves
        {
            get
            {
                return _displayLegalMoves;
            }
            set
            {
                _displayLegalMoves = value;
            }
        }

        public bool IllegalSound
        {
            get
            {
                return _illegalSound;
            }
            set
            {
                _illegalSound = value;
            }
        }

        public bool Gradient
        {
            get
            {
                return _gradient;
            }

            set
            {
                _gradient = value;
            }
        }

        public bool DarkMode
        {
            get
            {
                return _darkMode;
            }

            set
            {
                _darkMode = value;
            }
        }

        public bool BorderHighlight
        {
            get
            {
                return _borderHighlight;
            }
            set
            {
                _borderHighlight = value;
            }
        }

        public bool AutoCheck
        {
            get
            {
                return _autoCheck;
            }
            set
            {
                _autoCheck = value;
            }
        }

        public bool LocalMode
        {
            get
            {
                return _localMode;
            }
            set
            {
                _localMode = value;
            }
        }

        public bool HideOutput
        {
            get
            {
                return _hideOutput;
            }
            set
            {
                _hideOutput = value;
            }
        }

        public bool DisplayGridBorder
        {
            get
            {
                return _displayGridBorder;
            }
            set
            {
                _displayGridBorder = value;
            }
        }

        public bool IsMaximized
        {
            get
            {
                return _isMaximized;
            }
            set
            {
                _isMaximized = value;
            }
        }

        public bool Sound
        {
            get
            {
                return _sound;
            }
            set
            {
                _sound = value;
            }
        }

        public bool CheatMode
        {
            get
            {
                return _cheatMode;
            }

            set
            {
                _cheatMode = value;
            }
        }

        public int PgnAnalysisDepth
        {
            get
            {
                return _pgnAnalysisDepth;
            }
            set
            {
                _pgnAnalysisDepth = value;
            }
        }

        public int MultiPV
        {
            get
            {
                return _multiPV;
            }
            set
            {
                _multiPV = value;
            }
        }

        public int ClickDelay
        {
            get
            {
                return _clickDelay;
            }
            set
            {
                _clickDelay = value;
            }
        }

        public int SelectedFontIndex
        {
            get
            {
                return _selectedFontIndex;
            }

            set
            {
                _selectedFontIndex = value;
            }
        }

        public int AnimationTime
        {
            get
            {
                return _animTime;
            }
            set
            {
                _animTime = value;
            }
        }

        [XmlElement("EngineDarkSquare")]
        public int EngineDarkSquareAsArgb
        {
            get
            {
                return EngineDarkSquare.ToArgb();
            }
            set
            {
                EngineDarkSquare = Color.FromArgb(value);
            }
        }

        [XmlElement("EngineLightSquare")]
        public int EngineLightSquareAsArgb
        {
            get
            {
                return EngineLightSquare.ToArgb();
            }
            set
            {
                EngineLightSquare = Color.FromArgb(value);
            }
        }

        [XmlElement("BoardDarkSquare")]
        public int BoardDarkSquareAsArgb
        {
            get
            {
                return BoardDarkSquare.ToArgb();
            }
            set
            {
                BoardDarkSquare = Color.FromArgb(value);
            }
        }

        [XmlElement("BoardLightSquare")]
        public int BoardLightSquareAsArgb
        {
            get
            {
                return BoardLightSquare.ToArgb();
            }
            set
            {
                BoardLightSquare = Color.FromArgb(value);
            }
        }

        public int? SplitterDistance
        {
            get
            {
                return _splitterDistance;
            }
            set
            {
                _splitterDistance = value;
            }
        }

        public double PieceSizeFactor
        {
            get
            {
                return _sizeFactor;
            }
            set
            {
                _sizeFactor = value;
            }
        }

        public string LastPgnDir
        {
            get
            {
                return _lastPgnDir;
            }
            set
            {
                _lastPgnDir = value;
            }
        }

        public EngineMode EngineMode
        {
            get
            {
                return _engineMode;
            }
            set
            {
                _engineMode = value;
            }
        }

        public Level Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        public EngineList EngineList
        {
            get
            {
                return _engineList;
            }
            set
            {
                _engineList = value;
            }
        }

        public ChessFont SelectedChessFont
        {
            get
            {
                return _chessFonts[_selectedFontIndex];
            }
        }

        [XmlIgnore]
        public Color BoardDarkSquare { get; set; } = Color.FromArgb(140, 162, 173);

        [XmlIgnore]
        public Color BoardLightSquare { get; set; } = Color.FromArgb(222, 227, 230);

        [XmlIgnore]
        public Color EngineDarkSquare { get; set; } = Color.FromArgb(186, 85, 70);

        [XmlIgnore]
        public Color EngineLightSquare { get; set; } = Color.FromArgb(240, 216, 191);

        public List<ChessFont> ChessFonts
        {
            get
            {
                return _chessFonts;
            }

            set
            {
                _chessFonts = value;
            }
        }

        public Rectangle? Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
            }
        }

        #endregion

        private SerializedInfo()
        { }
    }
}

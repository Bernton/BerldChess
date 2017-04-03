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

        private bool _localMode;
        private bool _hideOutput;
        private bool _hideArrows;
        private bool _boardFlipped;
        private bool _displayGridBorder;
        private bool _isMaximized;
        private bool _cheatMode;
        private bool _sound;
        private int _engineTime;
        private Rectangle _bounds;
        private int _multiPV = 250;
        private int _animTime = 300;
        private List<ChessFont> _chessFonts = new List<ChessFont>();
        private int _selectedFontIndex = 0;
        private bool _autoCheck = false;
        private bool _darkMode = false;
        private int _clickDelay = 120;
        private double _sizeFactor = 0.9;

        #endregion

        #region Properties

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

        [XmlIgnore]
        public Color BoardDarkSquare { get; set; } = Color.FromArgb(140, 162, 173);

        [XmlIgnore]
        public Color BoardLightSquare { get; set; } = Color.FromArgb(222, 227, 230);

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

        [XmlIgnore]
        public Color EngineDarkSquare { get; set; } = Color.FromArgb(186, 85, 70);

        [XmlIgnore]
        public Color EngineLightSquare { get; set; } = Color.FromArgb(240, 216, 191);

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

        public int EngineTime
        {
            get
            {
                return _engineTime;
            }

            set
            {
                _engineTime = value;
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

        public Rectangle Bounds
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
        {  }
    }
}

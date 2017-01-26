using System;
using System.Drawing;

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

        private SerializedInfo() { }
    }
}

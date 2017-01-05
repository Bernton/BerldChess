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
        private Rectangle _bounds;


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

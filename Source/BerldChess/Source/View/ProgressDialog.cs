using System;
using System.Windows.Forms;

namespace BerldChess.View
{
    /// <summary>
    /// Provides a simple thread safe dialog which can show progress during a task.
    /// </summary>
    public class ProgressDialog : IDisposable
    {
        #region Events

        /// <summary>
        /// Occurs when the dialog has been closed via the user interface.
        /// </summary>
        public event EventHandler ClosedByInterface;

        #endregion

        #region Fields

        private FormProgressDialog _formProgressDialog;

        #endregion

        #region Properties

        public Control Parent
        {
            get
            {
                return _formProgressDialog.Parent;
            }
        }

        /// <summary>
        /// Gets and sets the percentage shown on the progress bar, the maximum value is 100.
        /// </summary>
        public int ProgressBarPercentage
        {
            get
            {
                return _formProgressDialog.ProgressBar.Value;
            }

            set
            {
                if (_formProgressDialog.InvokeRequired)
                {
                    _formProgressDialog.Invoke((MethodInvoker)delegate
                    {
                        _formProgressDialog.ProgressBar.Value = value;
                    });
                }
                else
                {
                    _formProgressDialog.ProgressBar.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the title shown on the dialog.
        /// </summary>
        public string Title
        {
            get
            {
                return _formProgressDialog.Text;
            }

            set
            {
                if (_formProgressDialog.InvokeRequired)
                {
                    _formProgressDialog.Invoke((MethodInvoker)delegate
                    {
                        _formProgressDialog.Text = value;
                    });
                }
                else
                {
                    _formProgressDialog.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the status text which is shown above the progress bar.
        /// </summary>
        public string StatusText
        {
            get
            {
                return _formProgressDialog.StatusLabel.Text;
            }

            set
            {
                if (_formProgressDialog.InvokeRequired)
                {
                    _formProgressDialog.Invoke((MethodInvoker)delegate
                    {
                        _formProgressDialog.StatusLabel.Text = value;
                    });
                }
                else
                {
                    _formProgressDialog.StatusLabel.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the style of the progress bar.
        /// </summary>
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                return _formProgressDialog.ProgressBar.Style;
            }

            set
            {
                if (_formProgressDialog.InvokeRequired)
                {
                    _formProgressDialog.Invoke((MethodInvoker)delegate
                    {
                        _formProgressDialog.ProgressBar.Style = value;
                    });
                }
                else
                {
                    _formProgressDialog.ProgressBar.Style = value;
                }
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of ProgressDialog.
        /// </summary>
        /// <param name="showCloseButton">Hides the close button on the dialog interface if set to false.</param>
        public ProgressDialog(bool showCloseButton = true)
        {
            InitializeForm(showCloseButton);
        }

        /// <summary>
        /// Creates a new instance of ProgressDialog with additional parameters.
        /// </summary>
        /// <param name="title">Sets the initial dialog title.</param>
        /// <param name="initialStatusText">Sets the initial status text.</param>
        /// <param name="showCloseButton">Hides the close button on the dialog interface if set to false.</param>
        public ProgressDialog(string title, string initialStatusText, bool showCloseButton = true)
        {
            InitializeForm(showCloseButton);

            Title = title;
            StatusText = initialStatusText;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Shows the dialog and blocks the current line until the dialog gets hidden/disposed.
        /// </summary>
        public void Show()
        {
            _formProgressDialog.ShowDialog();
        }

        /// <summary>
        /// Hides the dialog and sets the percentage of the progress bar back to 0.
        /// </summary>
        public void Hide()
        {
            ProgressBarPercentage = 0;

            if (_formProgressDialog.InvokeRequired)
            {
                _formProgressDialog.Invoke((MethodInvoker)delegate
                {
                    _formProgressDialog.Hide();
                });
            }
            else
            {
                _formProgressDialog.Hide();
            }
        }

        /// <summary>
        /// Disposes respectively destroys the dialog, making it unusable for further use.
        /// </summary>
        public void Dispose()
        {
            _formProgressDialog.Dispose();
        }

        /// <summary>
        /// Returns the current dialog title and status text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _formProgressDialog.Text + ": " + _formProgressDialog.StatusLabel.Text;
        }

        private void InitializeForm(bool showCloseButton)
        {
            _formProgressDialog = new FormProgressDialog();

            if (!showCloseButton)
            {
                _formProgressDialog.ControlBox = false;
            }

            _formProgressDialog.FormClosed += OnDialogFormClosed;
        }

        private void OnDialogFormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (ClosedByInterface != null)
                {
                    ClosedByInterface(this, e);
                }
            }
        }

        #endregion Methods
    }
}

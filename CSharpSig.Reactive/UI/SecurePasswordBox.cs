using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;

namespace ImprovingU.Reactive.UI
{
    /// <summary>
    ///   More secure PasswordBox based on TextBox control
    /// </summary>
    public class SecurePasswordBox : TextBox
    {
        // Fake char to display in Visual Tree
        const char PwdChar = '●';

        // flag used to bypass OnTextChanged
        bool _dirtyBaseText;

        /// <summary>
        ///   Only copy of real password
        /// </summary>
        /// <remarks>
        ///   For more security use System.Security.SecureString type instead
        /// </remarks>
        string _password = string.Empty;

        /// <summary>
        ///   Provide access to base.Text without call OnTextChanged
        /// </summary>
        protected string BaseText
        {
            get { return base.Text; }
            set
            {
                _dirtyBaseText = true;
                base.Text = value;
                _dirtyBaseText = false;
            }
        }

        /// <summary>
        ///   Clean Password
        /// </summary>
        public new string Text
        {
            get { return _password; }
            set
            {
                _password = value ?? string.Empty;
                BaseText = new string(PwdChar, _password.Length);
            }
        }

        /// <summary>
        ///   TextChanged event handler for secure storing of password into Visual Tree,
        ///   text is replaced with pwdChar chars, clean text is kept in
        ///   Text property (CLR property not snoopable without mod)
        /// </summary>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (_dirtyBaseText)
                return;

            string currentText = BaseText;

            int selStart = SelectionStart;
            if (_password != null && currentText.Length < _password.Length)
            {
                // Remove deleted chars
                _password = _password.Remove(selStart, _password.Length - currentText.Length);
            }
            if (!string.IsNullOrEmpty(currentText))
            {
                for (int i = 0; i < currentText.Length; i++)
                {
                    if (currentText[i] != PwdChar)
                    {
                        Debug.Assert(_password != null, "Password can't be null here");
                        // Replace or insert char
                        string currentCharacter = currentText[i].ToString(CultureInfo.InvariantCulture);
                        _password = BaseText.Length == _password.Length ? _password.Remove(i, 1).Insert(i, currentCharacter) : _password.Insert(i, currentCharacter);
                    }
                }
                Debug.Assert(_password != null, "Password can't be null here");
                BaseText = new string(PwdChar, _password.Length);
                SelectionStart = selStart;
            }
            base.OnTextChanged(e);
        }
    }
}

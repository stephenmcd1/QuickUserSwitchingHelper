using System;
using System.Windows.Forms;

namespace QuickUserSwitchingHelper
{
    /// <summary>
    /// A form that prompts for password input.
    /// </summary>
    public sealed partial class PasswordPrompt : Form
    {
        /// <summary>
        /// No comment necessary :-)
        /// </summary>
        private static readonly Keys[] Konami =
        {
            Keys.Up, Keys.Up,
            Keys.Down, Keys.Down,
            Keys.Left, Keys.Right,
            Keys.Left, Keys.Right,
            //Keys.B, Keys.A //We are already entering a password so don't want to interfere with that
        };

        /// <summary>
        /// How far along we are with Konami
        /// </summary>
        private int _konamiProgress;

        /// <summary>
        /// An optional string representing the actual password as encrypted by the pin
        /// </summary>
        private readonly string _secret;

        /// <summary>
        /// The password entered by the user into this window.
        /// </summary>
        public string Password { get; private set; }

        public PasswordPrompt(string windowTitle, string secret = null)
        {
            InitializeComponent();

            //Force the event to fire so we start off in a good state
            PasswordBoxOnTextChanged(PasswordBox, new EventArgs());

            //Try a bit to make sure we are the foreground app and have focus
            Load += (sender, args) => { Activate(); };

            Text = windowTitle;
            _secret = secret;
        }

        /// <summary>
        /// Try to derive the actual password based on the current entry (assuming it is a PIN matching our secret)
        /// </summary>
        /// <returns></returns>
        private string TryDerivePassword()
        {
            var pw = PasswordBox.Text;

            //See if they typed something that looks like a PIN and we have the Secret.  If so, try to decrypt it
            int pin;
            if (_secret != null && int.TryParse(pw, out pin))
            {
                return SecurityFunctions.TryDecrypt(_secret, pin);
            }
            return null;
        }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            //Try deriving the password using a PIN then fall-back to the entered password
            var pw = TryDerivePassword() ?? PasswordBox.Text;
            Password = pw;
            Close();
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PasswordBoxOnTextChanged(object sender, EventArgs e)
        {
            OkButton.Enabled = !string.IsNullOrWhiteSpace(PasswordBox.Text);

            //See if their entry is a valid PIN
            Password = TryDerivePassword();
            if (Password != null)
            {
                //If so, we're done
                Close();
            }
        }

        private void PasswordBoxOnKeyUp(object sender, KeyEventArgs e)
        {
            //If they got the next character right, progress.  Otherwise, reset back
            //NOTE: This intentionally disallows a sequence like [Up][Up][Up][Down]...
            //Logic taken from: http://stackoverflow.com/a/813201/385996
            _konamiProgress = (Konami[_konamiProgress] == e.KeyCode) ? ++_konamiProgress : 0;

            //If we have more to go, nothing else to do
            if (_konamiProgress != Konami.Length)
            {
                return;
            }

            //Otherwise, we got the code correct!

            //Reset our progress
            _konamiProgress = 0;

            //Prompt for a PIN - inception style using ourself
            using (var pw = new PasswordPrompt("Enter the pin"))
            {
                pw.ShowDialog();
                int pin;
                if (pw.Password == null || !int.TryParse(pw.Password, out pin))
                {
                    return;
                }
                var secret = SecurityFunctions.BuildSecret(PasswordBox.Text, pin);
                MessageBox.Show(this, secret, "Secret Data (Ctrl + C to copy to clipboard)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Cassia;

namespace QuickUserSwitchingHelper
{
    public static class UserSwitchingManager
    {
        public static void SwitchToUser(string userName, string password, string secret)
        {
            if (password == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var pwd = new PasswordPrompt("Enter password for " + userName, secret);
                Application.Run(pwd);
                password = pwd.Password;
                if (password == null)
                {
                    return;
                }
            }

            ITerminalServicesManager manager = new TerminalServicesManager();
            using (var server = manager.GetLocalServer())
            {
                server.Open();

                var currentSession = server.GetProcess(Process.GetCurrentProcess().Id).SessionId;
                var target = server.GetSessions().SingleOrDefault(s => string.Equals(s.UserName,userName, StringComparison.CurrentCultureIgnoreCase));
                if (target == null)
                {
                    //Not already logged on.  What to do?
                    return;
                }

                try
                {
                    server.GetSession(currentSession).Connect(target, password, true);
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message, "Error code: " + e.NativeErrorCode);
                }
            }
        }
    }
}

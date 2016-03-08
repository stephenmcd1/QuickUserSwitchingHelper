using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickUserSwitchingHelper
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            //Parse the command line options and then hand them off to the main logic
            var user = FindParm(args, "u");
            var pass = FindParm(args, "p");
            var secret = FindParm(args, "s");

            UserSwitchingManager.SwitchToUser(user, pass, secret);
        }

        /// <summary>
        /// Searches the given command line arguments for an item with the given prefix
        /// </summary>
        private static string FindParm(IEnumerable<string> args, string prefixLetter)
        {
            var prefix = $"/{prefixLetter}:";
            var match = args.FirstOrDefault(a => a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            return match?.Substring(prefix.Length);
        }
    }
}

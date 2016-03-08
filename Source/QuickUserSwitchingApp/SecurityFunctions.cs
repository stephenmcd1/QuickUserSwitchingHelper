using System;
using System.Security.Cryptography;
using System.Text;

namespace QuickUserSwitchingHelper
{
    /// <summary>
    /// Encapsulates all the security-related functions.  This class is in charge of the various encryption logic
    /// used to implemented the saved secret functionality.
    /// </summary>
    /// <remarks>
    /// We don't employ very rigorous encryption here since no matter what we do, it could be broken fairly easily.  Our
    /// only input is a four digit number so brute-forcing is not going to be difficult even if we employed some sort of
    /// key stretching functionality, etc.
    /// </remarks>
    public class SecurityFunctions
    {
        /// <summary>
        /// The encoding we'll use to convert the password to/from actual bytes.
        /// </summary>
        private static readonly Encoding PasswordEncoding = Encoding.Default;

        /// <summary>
        /// Use the given pin to attempt to decrypt the given secret value.  If the decryption is successful, the value is
        /// returned.  Otherwise, <c>null</c> is returned.
        /// </summary>
        /// <param name="base64Secret">A Base64 representation of the secret</param>
        /// <param name="pin">The pin to use for decryption</param>
        public static string TryDecrypt(string base64Secret, int pin)
        {
            try
            {
                //Try decrypting the secret using the PIN and return the resulting string (if possible)

                var bytes = ProtectedData.Unprotect(
                    Convert.FromBase64String(base64Secret),
                    GetBytesFromPin(pin),
                    DataProtectionScope.LocalMachine);

                return PasswordEncoding.GetString(bytes);
            }
            catch (CryptographicException)
            {
                //Invalid PIN, etc.
                return null;
            }
        }

        /// <summary>
        /// Builds a Base64 representation string that can be decrypted with the given pin to result in the password.
        /// </summary>
        public static string BuildSecret(string password, int pin)
        {
            var bytes = ProtectedData.Protect(
                PasswordEncoding.GetBytes(password),
                GetBytesFromPin(pin),
                DataProtectionScope.LocalMachine);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Gets a byte[] representation of the pin for en/decryption use
        /// </summary>
        private static byte[] GetBytesFromPin(int pin)
        {
            return Encoding.ASCII.GetBytes(pin.ToString());
        }
    }
}

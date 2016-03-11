# Quick User Switching Helper

##Summary

Allows you to quickly switch to a specific Windows user with a single click.

##Usage

###Command Line Options

The primary means of using this application is by launching it with specific command line options.  The available command line options are:
 - /u - Required.  The specific user account to connect to
 - /p - Optional.  The password to use to connect to the user account.  If ommitted, the application will bring up a password prompt.
 - /s - Optional.  Specifies a 'secret' value that allows switching users via a PIN instead of the full password.  See below for more details on this option.

###PIN Mode

If you don't want to hard-code your password as a command line option but still want a semi-quick way of switching users without having to type your full password, this mode provides a good middle ground.  You provide the PIN and your full password and the tool will create an encrypted value of your password (called the 'secret' value) that can only be decrypted with the PIN.

Please note that since the PIN is only 4 digits long, you shouldn't expect a great deal of security when using this.  Brute forcing the PIN is very much possible.  The goal of this mode is to at least obfuscate your password and make it moderately hard for a casual person to crack.

To setup PIN mode:

1. Launch the app as normal.
2. Enter your full password but stay on the password prompt window
3. Type the [Konami code](https://en.wikipedia.org/wiki/Konami_Code) (but without the <kbd>A</kbd> and <kbd>B</kbd> keys since that would interfere with password entry).
4. Enter the pin you want to use
5. You'll be given the 'secret' value.  You can pass this with the /s command line option

##Credits

This application makes use of the following open source projects:

 - [Fody](https://github.com/Fody/Fody) / [Costura](https://github.com/Fody/Costura) - Allows the application to be built as a single, standalone .exe file (.Net Framework must still be installed)
 - [Cassia](https://code.google.com/archive/p/cassia/) - A managed interface to the Windows Terminal Services API

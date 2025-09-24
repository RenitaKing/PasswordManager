using System;
using System.Collections.Generic;
using SplashKitSDK;


namespace PasswordManager
{
    public class AppController
    {
        private readonly AuthenticationService _authService;
        private readonly PasswordStore _store;
        private readonly EncryptionService _crypto;
        public Window window = new Window("Password Manager", 800, 600);

        // Login state
        private bool authenticated = false;
        private string _masterPassword = "";
        private string _loggedPassword = "";

        public AppController(AuthenticationService authService, PasswordStore store, EncryptionService crypto)
        {
            _authService = authService;
            _store = store;
            _crypto = crypto;

        }


        public void Run()
        {
            // Step 0: First-time setup if no master password exists
            if (!_authService.HasMasterPassword)
            {
                string newPassword = ShowCreateMasterPasswordScreen();
                _authService.CreateMasterPassword(newPassword);
            }

            //Step 1: Show the login screen and test that succeeds
            var login = new LoginScreen(_authService, window);
            RunScreen(login);

            // If user closed the window during login, just exit
            if (!login.Success || window.CloseRequested) return;

            // Step 2: Configure crypto & load store
            authenticated = true;
            _masterPassword = login.Password;
            _loggedPassword = login.Password; //store this for later, in case change password requested.
            _store.ConfigureCrypto(_crypto, _masterPassword!);
            _store.LoadFromFile();

            //Step 3:  Main Password Screen
            var passwordScreen = new PasswordScreen(window, _store);

            while (!window.CloseRequested && !passwordScreen.QuitRequested)
            {
                RunScreen(passwordScreen);
                //Check if the password change request occurred
                if (passwordScreen.ChangeMasterPasswordRequested)
                {
                    ShowChangeMasterPasswordScreen();
                    passwordScreen.ChangeMasterPasswordRequested = false; // reset
                }
            }
            // Save on exit
            if (authenticated)
                _store.SaveToFile();

            SplashKit.CloseAllWindows();
        }

        //Basic RunScreen function:

        private void RunScreen(ScreenBase screen)
        {
            screen.IsComplete = false;

            while (!window.CloseRequested && !screen.IsComplete)
            {
                SplashKit.ProcessEvents();
                window.Clear(SplashKitSDK.Color.White);

                screen.Draw();
                screen.HandleInput();

                window.Refresh();
            }
        }
        // Handles first-time password setup (UI prompt)
        private string ShowCreateMasterPasswordScreen()
        {
            var screen = new CreatePasswordScreen(_authService, window);
            RunScreen(screen);
            return screen.Password;
        }
        private void ShowChangeMasterPasswordScreen()
        {
            var screen = new ChangePasswordScreen(_authService, window);
            RunScreen(screen);

            if (screen.Success)
            {
                // Update master in memory
                _masterPassword = screen.NewPassword;
                // Re-encrypt everything with new password
                _store.ReencryptAll(_crypto, _loggedPassword, _masterPassword);
                //Update the loggedPassword
                _loggedPassword = _masterPassword;
            }
        }




    }
}

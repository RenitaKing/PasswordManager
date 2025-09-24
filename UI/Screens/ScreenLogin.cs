using System;
using System.Runtime.CompilerServices;
using SplashKitSDK;

namespace PasswordManager
{
    public class LoginScreen : LogoFormScreen
    {
        public bool Success { get; private set; } = false;
        private LoginForm _loginForm;
        public string Password="";
        
        public LoginScreen(AuthenticationService authService, Window window) : base(authService, window)
        {
            var (x, y, w) = GetDefaultFormPlacement();
            _loginForm = new LoginForm(window,x,y,w);
            //Subscribe to our events
            _loginForm.OnLoginClicked += AttemptLogin;
            _loginForm.OnEnterPressed += AttemptLogin;
            _form = _loginForm;

            _uiElements.AddRange(_form.GetElements());
            RefreshFocusableElements();
        }
        private void AttemptLogin()
        {
            string pwd = _loginForm.Password;

            if (string.IsNullOrEmpty(pwd))
            {
                _loginForm.ShowError("Please enter a password");
                return;
            }

            if (_authService.Authenticate(pwd))
            {
                Success = true;
                IsComplete = true;
                Password = pwd;
            }
            else
            {
                _loginForm.ShowError("Incorrect Password");
            }
        }


    }
}

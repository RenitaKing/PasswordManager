using SplashKitSDK;

namespace PasswordManager
{
    public class CreatePasswordScreen : LogoFormScreen
    {
        public bool Success { get; private set; } = false;
        private CreatePWForm _createForm;
        public string Password { get; private set; } = "";

        public CreatePasswordScreen(AuthenticationService authService, Window window)
            : base(authService,window)
        {
            var (x, y, w) = GetDefaultFormPlacement();
            _createForm = new CreatePWForm(window, x,y,w);
            //Subscribe to our events
            _createForm.OnEnterPressed += AttemptCreate;
            _createForm.OnLoginClicked += AttemptCreate;
            _form = _createForm;

            _uiElements.AddRange(_form.GetElements());
            RefreshFocusableElements();
        }

        private void AttemptCreate()
        {
            string pwd = _createForm.Password;
            string pwdCheck = _createForm.ConfirmPassword;
            if (pwd != pwdCheck)
            {
                _createForm.ShowError("Passwords do not match");
                return;
            }

            _authService.CreateMasterPassword(pwd);
            Password = pwd;
            Success = true;
            IsComplete = true;
        }
    }
}

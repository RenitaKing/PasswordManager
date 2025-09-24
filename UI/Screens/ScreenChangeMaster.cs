using SplashKitSDK;

namespace PasswordManager
{
    public class ChangePasswordScreen : LogoFormScreen
    {
        public string NewPassword { get; private set; } = "";
        private ChangePWForm _changeForm;
        public bool Success { get; private set; } = false;

        public ChangePasswordScreen(AuthenticationService authService, Window window)
            : base(authService, window)
        {

            var (x, y, w) = GetDefaultFormPlacement(width:300);
            //Create our form
            _changeForm = new ChangePWForm(window, x,y,w);
            //Subscribe to the events
            _changeForm.OnChangeClicked += AttemptChange;
            _changeForm.OnEnterPressed += AttemptChange;
            _changeForm.OnCancelClicked += CancelChange;

            _form = _changeForm;

            _uiElements.AddRange(_form.GetElements());
            RefreshFocusableElements();
        }
        
        private void AttemptChange()
        {
            string pwdOld = _changeForm.OldPassword;
            string pwdNew = _changeForm.NewPassword;
            string pwdCheck = _changeForm.ConfirmPassword;
            if (!_authService.Authenticate(pwdOld))
            {
                _changeForm.ShowError("Old password incorrect");
                return;
            }

            if (pwdNew != pwdCheck)
            {
                _changeForm.ShowError("Passwords do not match");
                return;
            }

            _authService.ChangeMasterPassword(pwdNew);
            Success = true;
            NewPassword = pwdNew;
            IsComplete = true;

        }
        private void CancelChange()
        {

            IsComplete = true;

        }
    }
}

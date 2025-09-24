using SplashKitSDK;
using System.Collections.Generic;


namespace PasswordManager
{
    public class ChangePWForm : FormPanel
    {
        private readonly TextField _oldPasswordBox = null!;
        private readonly TextField _newPasswordBox = null!;
        private readonly TextField _confirmPasswordBox = null!;
        public LabelField ErrorLabel { get; set; } = null!;

        public event Action? OnChangeClicked;
        public event Action? OnCancelClicked;
        public event Action? OnEnterPressed;
        public string OldPassword => _oldPasswordBox.Text;
        public string NewPassword => _newPasswordBox.Text;
        public string ConfirmPassword => _confirmPasswordBox.Text;

        public ChangePWForm(Window window, int x, int y, int width, int height = 200, int spacing = 8)
            : base(window, x, y, width, spacing)
        {
            SetHeader("Welcome! Enter Master Password");

            // Add fields
            AddLabelledField(ref _oldPasswordBox, "Current Password:");
            _oldPasswordBox.HasFocus = true;
            _oldPasswordBox.Mask = true;
            AddLabelledField(ref _newPasswordBox, "New Password:");
            _newPasswordBox.Mask = true;
            AddLabelledField(ref _confirmPasswordBox, "Confirm New Password:");
            _confirmPasswordBox.Mask = true;

            // Add buttons under ConfirmPassword box
            AddMultiButtonUnderField(
                _confirmPasswordBox,
                new (string, Action<Button>)[]
                {
                    ("Change Password", b => {OnChangeClicked?.Invoke();}),
                    ("Cancel", b => {OnCancelClicked?.Invoke();})
                },
                rounded: false
                );
            ErrorLabel = AddLabel("");

        }
        public override void HandleInput()
        {
            base.HandleInput();

            if (_confirmPasswordBox.HasFocus && SplashKit.KeyTyped(KeyCode.ReturnKey))
            {
                OnEnterPressed?.Invoke();
            }
        }

        public void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.TextColor = SplashKitSDK.Color.SwinburneRed;
        }





    }
}

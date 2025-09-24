using SplashKitSDK;
using System.Collections.Generic;


namespace PasswordManager
{
    public class CreatePWForm : FormPanel
    {
        private readonly TextField _newPasswordBox = null!;
        private readonly TextField _confirmPasswordBox = null!;
        public Button CreateButton { get; private set; }
        public LabelField ErrorLabel { get; set; } = null!;
        
        public event Action? OnLoginClicked;
        public event Action? OnEnterPressed;
        public string Password => _newPasswordBox.Text;
        public string ConfirmPassword => _confirmPasswordBox.Text;

        public CreatePWForm(Window window, int x, int y, int width, int height = 200, int spacing = 8)
            : base(window, x, y, width, spacing)
        {
            SetHeader("Welcome! Enter Master Password");

            // Add fields
            AddLabelledField(ref _newPasswordBox, "Master Password:");
            _newPasswordBox.HasFocus = true;
            _newPasswordBox.Mask = true;
            AddLabelledField(ref _confirmPasswordBox, "Confirm Password:");
            _confirmPasswordBox.Mask = true;

            // Add Login buttons under ConfirmPassword box
            CreateButton = AddButtonUnderField(_confirmPasswordBox, "Create Password Key", rounded: false);
            CreateButton.OnClick += (btn) => OnLoginClicked?.Invoke();
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

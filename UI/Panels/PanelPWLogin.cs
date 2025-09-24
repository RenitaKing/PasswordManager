using SplashKitSDK;
using System.Collections.Generic;


namespace PasswordManager
{
    public class LoginForm : FormPanel
    {
        private TextField _passwordBox = null!;
        public LabelField ErrorLabel { get; set; } = null!;
        public Button LoginButton { get; private set; }
        public event Action? OnLoginClicked;
        public event Action? OnEnterPressed;
        public string Password => _passwordBox.Text;

        public LoginForm(Window window, int x, int y, int width, int height = 200, int spacing = 8)
            : base(window, x, y, width, spacing)
        {
            SetHeader("Welcome! Enter Master Password");

            // Add fields
            AddLabelledField(ref _passwordBox, "Master Password:");
            _passwordBox.HasFocus = true;
            _passwordBox.Mask = true;

            // Add Login buttons under Password box
            LoginButton = AddButtonUnderField(_passwordBox, "Enter", rounded: false);
            LoginButton.OnClick += (btn) => OnLoginClicked?.Invoke();
            ErrorLabel = AddLabel("");

        }
        public override void HandleInput()
        {
            base.HandleInput();

            if (_passwordBox.HasFocus && SplashKit.KeyTyped(KeyCode.ReturnKey))
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

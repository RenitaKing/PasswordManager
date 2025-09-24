using SplashKitSDK;

namespace PasswordManager
{
    // Shared base for screens that show a logo and a form panel
    public abstract class LogoFormScreen : ScreenBase
    {
        protected readonly AuthenticationService _authService;
        protected ImageElement _logo;
        protected FormPanel? _form;

        protected LogoFormScreen(AuthenticationService authService, Window window) : base(window)
        {
            _authService = authService;
            // Use the logo from UIResources
            _logo = new ImageElement(window, (_window.Width - UIResources.Logo.Width) / 2, 50, UIResources.Logo);

        }

        public override void Draw()
        {
            _window.Clear(SplashKitSDK.Color.White);
            _logo.Draw();
            _form?.Draw();

        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        protected (int x, int y, int width) GetDefaultFormPlacement(int offsetY = 50, int width = 200)
        {
            int centerX = _window.Width / 2;
            int startY = _window.Height-220;
            return (centerX - width / 2, startY, width);
        }

    }
}

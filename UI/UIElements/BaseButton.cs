using System;
using SplashKitSDK;

namespace PasswordManager
{
    public class Button : UIElement
    {
        public string Caption { get; set; } = "";
        public int FontSize { get; set; } = UIConfig.DefaultFontSize;
        public ActionButtonType? Type { get; private set; } = null;
        public bool NeedsWarning { get; set; } = false;
        protected SplashKitSDK.Color BaseColour;
        protected SplashKitSDK.Color HoverColour;
        public event Action<Button>? OnClick;

        public Button(Window window, int x, int y, int width, int height,
                      string caption, int fontSize = 0, ActionButtonType? type = null)
            : base(window, x, y, width, height)
        {
            Caption = caption;
            if (fontSize != 0) FontSize = fontSize;  // override only if nonzero

            Type = type;

            BaseColour = UIConfig.ButtonBaseColour;
            HoverColour = UIConfig.ButtonHoverColour; // default
        }

        public override void Draw()
        {
            var mouse = SplashKit.MousePosition();
            bool isHover = mouse.X >= X && mouse.X <= X + Width &&
                           mouse.Y >= Y && mouse.Y <= Y + Height;

            SplashKitSDK.Color bg = isHover
                ? (NeedsWarning ? UIConfig.ButtonWarningHoverColour : UIConfig.ButtonHoverColour)
                : BaseColour;

            // Regular rectangle
            _window.FillRectangle(bg, X, Y, Width, Height);
            _window.DrawRectangle(UIConfig.ActionButtonBorderColour, X, Y, Width, Height);

            var rect = new SplashKitSDK.Rectangle { X = X, Y = Y, Width = Width, Height = Height };
            DrawCentredText(Caption, UIConfig.ButtonTextColour, FontSize, rect, font: UIConfig.ActionButtonFont);
        }

        public override void HandleInput()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                var mouse = SplashKit.MousePosition();
                if (mouse.X >= X && mouse.X <= X + Width &&
                    mouse.Y >= Y && mouse.Y <= Y + Height)
                {
                    OnClick?.Invoke(this);
                }
            }
        }
    }
}

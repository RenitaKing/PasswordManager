using System;
using SplashKitSDK;

namespace PasswordManager
{
    public class RoundedButton : Button
    {
        private int _radius = 3;
        private ActionButtonType? _actionType;
        private SplashKitSDK.Color _baseColor;
        private SplashKitSDK.Color _hoverColor;

        public RoundedButton(Window window, int x, int y, int width, int height, string caption,
                             ActionButtonType? actionType = null, int radius = 3)
            : base(window, x, y, width, height, caption)
        {
            _radius = UIConfig.DefaultButtonRadius;

_baseColor = UIConfig.ButtonBaseColour;

_hoverColor = actionType switch
{
    ActionButtonType.Delete => UIConfig.ButtonWarningHoverColour,
    _ => UIConfig.ButtonHoverColour
};

            _actionType = actionType;

            
        }

        public override void Draw()
        {
            var mouse = SplashKit.MousePosition();
            bool isHover = mouse.X >= X && mouse.X <= X + Width &&
                           mouse.Y >= Y && mouse.Y <= Y + Height;

            SplashKitSDK.Color bg = isHover ? _hoverColor : _baseColor;

            // Draw rectangles
            _window.FillRectangle(bg, X + _radius, Y, Width - 2 * _radius, Height);
            _window.FillRectangle(bg, X, Y + _radius, Width, Height - 2 * _radius);

            // Draw ends
            _window.FillCircle(bg, X+_radius, Y + _radius, _radius);//Left
            _window.FillCircle(bg, X + Width - _radius, Y + _radius, _radius);//right
            _window.FillCircle(bg, X + _radius, Y + Height - _radius-1, _radius);
            _window.FillCircle(bg, X + Width - _radius, Y + Height - _radius-1, _radius);

            // Draw text centered
            var rect = new SplashKitSDK.Rectangle { X = X, Y = Y, Width = Width, Height = Height };
            DrawCentredText(Caption, UIConfig.ButtonTextColour, FontSize, rect, font: UIConfig.ActionButtonFont);
        }
    }

}

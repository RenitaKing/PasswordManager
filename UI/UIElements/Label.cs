using SplashKitSDK;

namespace PasswordManager
{
    public class LabelField : UIElement
    {
        public string Text { get; set; }
        public SplashKitSDK.Color TextColor { get; set; } = UIConfig.LabelFontColour;
        public int FontSize { get; set; } = UIConfig.LabelFontSize;
        public Font LabelFont { get; set; } = UIConfig.DefaultFont;

        public LabelField(Window window, int x, int y, string text)
            : base(window, x, y,
                SplashKit.TextWidth(text, UIConfig.DefaultFont, UIConfig.LabelFontSize),
                SplashKit.TextHeight(text, UIConfig.DefaultFont, UIConfig.LabelFontSize))
        {
            Text = text;
            CanFocus = false; // labels shouldn’t receive focus
        }

        public override void Draw()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                _window.DrawText(Text, TextColor, LabelFont, FontSize, X, Y);
            }
        }

        public override void HandleInput()
        {
            // Labels don’t handle input
        }
    }
}

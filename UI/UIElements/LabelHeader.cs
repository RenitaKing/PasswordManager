using System.Reflection.Emit;
using SplashKitSDK;

namespace PasswordManager
{
    public class HeaderField : LabelField
    {
        public HeaderField(
            Window window,
            int x,
            int y,
            string text)
            : base(window, x, y, text)
        {
                // Override the defaults from LabelField
            TextColor = UIConfig.HeaderFontColour;
            FontSize = UIConfig.HeaderFontSize;
            }
    }
}
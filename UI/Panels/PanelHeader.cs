using SplashKitSDK;
using System;
using System.Collections.Generic;

namespace PasswordManager
{
    public class EntryListHeader : Panel
    {
        private string _text;

        public EntryListHeader(Window window, int x, int y, int width, int height, string text)
            : base(window, x, y, width, height)
        {
            _text = text;
        }

        public override void Draw()
        {
            _window.DrawText(_text, UIConfig.HeaderFontColour, UIConfig.DefaultFont, UIConfig.HeaderFontSize, X + 5, Y + 5);
        }
    }
}

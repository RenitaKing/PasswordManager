using SplashKitSDK;
using System;

namespace PasswordManager
{
    public class EntryButton : Button
    {
        private readonly PasswordEntry _entry;

        public PasswordEntry Entry => _entry;
        public bool IsSelected { get; set; } = false;
        public string? HighlightTerm { get; set; }

        // Shadow the base event with a more specific type
        public new event Action<EntryButton>? OnClick;

        public EntryButton(Window window, int x, int y, int width, int height, PasswordEntry entry)
            : base(window, x, y, width, height, $"{entry.Website} ({entry.Username}) \n{entry.Category}")
        {
            _entry = entry;
            // Set up our own event for the entry button
            base.OnClick += (b) => OnClick?.Invoke(this);

        }

        public override void Draw()
        {
            // background
            SplashKitSDK.Color bg = IsSelected 
                        ? SplashKitSDK.Color.LightBlue 
                        : SplashKitSDK.Color.White;

            _window.FillRectangle(bg, X, Y, Width, Height);
            _window.DrawRectangle(UIConfig.ActionButtonBorderColour, X, Y, Width, Height);

            DrawMultilineHighlightText(
                Caption,
                UIConfig.ButtonEntryColour,
                HighlightTerm,
                UIConfig.DefaultFontSize,
                X + 5,
                Y + 5,
                UIConfig.DefaultFont
                );
        }
    }
}
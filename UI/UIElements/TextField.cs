using System;
using System.Collections.Generic;
using SplashKitSDK;


namespace PasswordManager
{
    public class TextField : UIElement
    {
        public string Text { get; private set; } = "";
        public bool Mask { get; set; } = false;
        public bool AllowMultiline { get; set; } = false;
        private uint _lastTick;
        private bool _showCursor = true;
        public bool IsSelected { get; private set; } = false;
        private SplashKitSDK.Timer _timer = new SplashKitSDK.Timer("Cursor");

        public TextField(Window window, int x, int y, int width, int height, bool mask = false)
            : base(window, x, y, width, height)
        {
            Mask = mask;
        }

        public void SetText(string text)
        {
            Text = text;
        }
        public void SelectAll()
        {
            IsSelected = true;
            HasFocus = true;
        }


        public void ClearSelection()
        {
            IsSelected = false;
        }


        public override void Draw()
        {
            DrawBackground();
            DrawText();
            DrawCursor();
        }


        private void DrawBackground()
        {
            SplashKitSDK.Color bgColour = HasFocus ? UIConfig.TextFocusColour : UIConfig.TextBaseColour;
            _window.FillRectangle(bgColour, X, Y, Width, Height);
            _window.DrawRectangle(UIConfig.TextBorderColour, X, Y, Width, Height);
        }


        private void DrawText()
        {
            string displayText = Mask ? new string('*', Text.Length) : Text;

            if (IsSelected && !string.IsNullOrEmpty(displayText))
            {
                // Fill a blue rectangle behind the text
                int textWidth = SplashKit.TextWidth(displayText, UIConfig.DefaultFont, UIConfig.DefaultFontSize);
                int textHeight = SplashKit.TextHeight(displayText, UIConfig.DefaultFont, UIConfig.DefaultFontSize);

                _window.FillRectangle(
                    SplashKitSDK.Color.LightBlue,
                    X + 5,
                    Y + 5,
                    textWidth,
                    textHeight
                );

                _window.DrawText(displayText, SplashKitSDK.Color.White, UIConfig.DefaultFont, UIConfig.DefaultFontSize, X + 5, Y + 5);
            }
            else
            {
                DrawWrappedHighlightText(
                    displayText,
                    UIConfig.TextFontColour,
                    "",
                    UIConfig.DefaultFontSize,
                    X + 5,
                    Y + 5,
                    Width - 10,
                    UIConfig.DefaultFont
                );
            }
        }


        private void DrawCursor()
        {
            if (HasFocus && _showCursor)
            {
                string displayText = Mask ? new string('*', Text.Length) : Text;
                float cursorX = X + 5 + SplashKit.TextWidth(displayText, UIConfig.DefaultFont, UIConfig.DefaultFontSize);
                _window.DrawLine(
                    SplashKitSDK.Color.DimGray,
                    cursorX,
                    Y + 5,
                    cursorX,
                    Y + SplashKit.TextHeight(displayText, UIConfig.DefaultFont, UIConfig.DefaultFontSize) + 3
                );
            }

        }

        public override void HandleInput()
        {
            UpdateCursor();
            HandleMouseFocus();

            if (HasFocus)
                HandleKeyboardInput();
        }


        private void HandleMouseFocus()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                HasFocus = SplashKit.PointInRectangle(
                    SplashKit.MousePosition(),
                    new SplashKitSDK.Rectangle() { X = X, Y = Y, Width = Width, Height = Height }
                );
            }
        }


        private void HandleKeyboardInput()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (SplashKit.KeyTyped(key))
                {
                    if (AllowMultiline && key == KeyCode.ReturnKey)
                    {
                        Text += "\n";
                    }
                    else if (key == KeyCode.BackspaceKey && Text.Length > 0)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                    else
                    {
                        string typedChar = KeyToChar(key);
                        if (!string.IsNullOrEmpty(typedChar))
                            Text += typedChar;
                    }
                }
            }
        }

        private string KeyToChar(KeyCode key)
        {
            if (TryGetLetter(key, out string letter)) return letter;
            if (TryGetNumber(key, out string number)) return number;
            if (TryGetPunctuation(key, out string punct)) return punct;
            return "";
        }

        private bool TryGetLetter(KeyCode key, out string result)
        {
            result = "";
            if (key >= KeyCode.AKey && key <= KeyCode.ZKey)
            {
                string letter = key.ToString().Substring(0, 1);
                result = SplashKit.KeyDown(KeyCode.LeftShiftKey) || SplashKit.KeyDown(KeyCode.RightShiftKey)
                    ? letter.ToUpper()
                    : letter.ToLower();
                return true;
            }
            return false;
        }

        private bool TryGetNumber(KeyCode key, out string result)
        {
            var normal = new Dictionary<KeyCode, string> {
                {KeyCode.Num0Key,"0"}, {KeyCode.Num1Key,"1"}, {KeyCode.Num2Key,"2"},
                {KeyCode.Num3Key,"3"}, {KeyCode.Num4Key,"4"}, {KeyCode.Num5Key,"5"},
                {KeyCode.Num6Key,"6"}, {KeyCode.Num7Key,"7"}, {KeyCode.Num8Key,"8"}, {KeyCode.Num9Key,"9"},
            };
            var shifted = new Dictionary<KeyCode, string> {
                {KeyCode.Num0Key,")"}, {KeyCode.Num1Key,"!"}, {KeyCode.Num2Key,"@"},
                {KeyCode.Num3Key,"#"}, {KeyCode.Num4Key,"$"}, {KeyCode.Num5Key,"%"},
                {KeyCode.Num6Key,"^"}, {KeyCode.Num7Key,"&"}, {KeyCode.Num8Key,"*"}, {KeyCode.Num9Key,"("},
            };


            if (normal.ContainsKey(key))
            {
                result = SplashKit.KeyDown(KeyCode.LeftShiftKey) || SplashKit.KeyDown(KeyCode.RightShiftKey)
                    ? shifted[key]
                    : normal[key];
                return true;
            }

            result = "";
            return false;
        }


        private bool TryGetPunctuation(KeyCode key, out string result)
        {
            bool shift = SplashKit.KeyDown(KeyCode.LeftShiftKey) || SplashKit.KeyDown(KeyCode.RightShiftKey);
            var punct = new Dictionary<KeyCode, string>
            {
                { KeyCode.SpaceKey," " },
                {KeyCode.PeriodKey,shift?">":"."},
                {KeyCode.CommaKey,shift?"<":","},
                {KeyCode.SemiColonKey,shift?":":";"},
                {KeyCode.QuoteKey,shift?"\"":"'"},
                {KeyCode.SlashKey,shift?"?":"/"},
                {KeyCode.BackslashKey,shift?"|":"\\"},
                {KeyCode.LeftBracketKey,shift?"{":"["},
                {KeyCode.RightBracketKey,shift?"}":"]"},
                {KeyCode.MinusKey,shift?"_":"-"},
                {KeyCode.EqualsKey,shift?"+":"="},
            };


            if (punct.ContainsKey(key))
            {
                result = punct[key];
                return true;
            }

            result = "";
            return false;
        }

        private void UpdateCursor()
        {
            //Update cursor
            uint now = _timer.Ticks;
            if (now - _lastTick >= 500)
            {
                _showCursor = !_showCursor;
                _lastTick = now;
            }
        }
    }
}

using System.Security.Cryptography.X509Certificates;
using SplashKitSDK;
using static SplashKitSDK.SplashKit;

namespace PasswordManager
{
    public abstract class UIElement
    {
        protected Window _window;
        protected Font _font;
        public int X { get; set; }

        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool CanFocus { get; protected set; } = true;
        public bool HasFocus { get; set; } = false;


        protected UIElement(Window window, int x = 0, int y = 0, int width = 0, int height = 0)
        {
            _window = window;
            _font = UIResources.Arial;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public abstract void Draw();
        public abstract void HandleInput();

        protected void DrawCentredText(
                string text,
                SplashKitSDK.Color textColor,
                int fontSize,
                SplashKitSDK.Rectangle? rect = null,
                int yOffset = 0,
                Font? font = null)
        {
            if (string.IsNullOrEmpty(text)) return;

            // Use provided font, otherwise fallback to _font
            Font drawFont = font ?? _font;


            // Split into lines if text contains newlines
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            int totalHeight = 0;
            int[] lineHeights = new int[lines.Length];

            // Measure each line
            for (int i = 0; i < lines.Length; i++)
            {
                int h = SplashKit.TextHeight(lines[i], drawFont, fontSize);
                lineHeights[i] = h;
                totalHeight += h;
            }

            int baseX, baseY;

            if (rect.HasValue)
            {
                baseX = (int)rect.Value.X;
                baseY = (int)(rect.Value.Y + (rect.Value.Height - totalHeight) / 2 + yOffset);
            }
            else
            {
                baseX = 0;
                baseY = (_window.Height - totalHeight) / 2 + yOffset;
            }

            // Draw each line centered horizontally
            int currentY = baseY;
            foreach (var line in lines)
            {
                int textWidth = SplashKit.TextWidth(line, drawFont, fontSize);
                int x = (int)(rect.HasValue
                    ? baseX + (rect.Value.Width - textWidth) / 2
                    : (_window.Width - textWidth) / 2);

                _window.DrawText(line, textColor, drawFont, fontSize, x, currentY);
                currentY += SplashKit.TextHeight(line, drawFont, fontSize); // move down for next line
            }
        }
        protected void DrawMultilineHighlightText(
                string text,
                SplashKitSDK.Color defaultColor,
                string? highlightTerm,
                int fontSize,
                int startX,
                int startY,
                Font? font = null,
                int lineSpacing = 2)
            {
            if (string.IsNullOrEmpty(text)) return;
            Font drawFont = font ?? _font;

            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int offsetY = startY;

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(highlightTerm) &&
                    line.IndexOf(highlightTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int idx = line.IndexOf(highlightTerm, StringComparison.OrdinalIgnoreCase);
                    string before = line.Substring(0, idx);
                    string match = line.Substring(idx, highlightTerm.Length);
                    string after = line.Substring(idx + highlightTerm.Length);

                    int offsetX = startX;

                    // draw before
                    _window.DrawText(before, defaultColor, drawFont, fontSize, offsetX, offsetY);
                    offsetX += SplashKit.TextWidth(before, drawFont, fontSize);

                    // draw highlight
                    _window.FillRectangle(SplashKitSDK.Color.Gold, offsetX, offsetY, SplashKit.TextWidth(match, drawFont, fontSize), SplashKit.TextHeight(match, drawFont, fontSize));
                    _window.DrawText(match, defaultColor, drawFont, fontSize, offsetX, offsetY);
                    offsetX += SplashKit.TextWidth(match, drawFont, fontSize);

                    // draw after
                    _window.DrawText(after, defaultColor, drawFont, fontSize, offsetX, offsetY);
                }
                else
                {
                    _window.DrawText(line, defaultColor, drawFont, fontSize, startX, offsetY);
                }

                offsetY += SplashKit.TextHeight(line, drawFont, fontSize) + lineSpacing;
            }
        }

        protected void DrawWrappedHighlightText(
            string text,
            SplashKitSDK.Color defaultColor,
            string? highlightTerm,
            int fontSize,
            int startX,
            int startY,
            int maxWidth,
            Font? font = null,
            int lineSpacing = 2)
        {
            Font drawFont = font ?? _font;
            int offsetY = startY;

            // Split into paragraphs on \n
            foreach (string paragraph in text.Split('\n'))
            {
                string line = "";
                string[] words = paragraph.Split(' ');

                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                    int width = SplashKit.TextWidth(testLine, drawFont, fontSize);

                    if (width > maxWidth)
                    {
                        // Draw current line
                        DrawHighlightLine(line, defaultColor, highlightTerm, fontSize, startX, offsetY, drawFont);
                        offsetY += SplashKit.TextHeight(line, drawFont, fontSize) + lineSpacing;
                        line = word; // start new line
                    }
                    else
                    {
                        line = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(line))
                {
                    DrawHighlightLine(line, defaultColor, highlightTerm, fontSize, startX, offsetY, drawFont);
                    offsetY += SplashKit.TextHeight(line, drawFont, fontSize) + lineSpacing;
                }

                // Preserve blank lines
                if (paragraph.Length == 0) offsetY += SplashKit.TextHeight("A", drawFont, fontSize) + lineSpacing;
            }
        }

        private void DrawHighlightLine(
            string line,
            SplashKitSDK.Color defaultColor,
            string? highlightTerm,
            int fontSize,
            int startX,
            int startY,
            Font font)
        {
            if (!string.IsNullOrWhiteSpace(highlightTerm))
            {
                int idx = line.IndexOf(highlightTerm, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    string before = line.Substring(0, idx);
                    string match = line.Substring(idx, highlightTerm.Length);
                    string after = line.Substring(idx + highlightTerm.Length);

                    int offsetX = startX;

                    _window.DrawText(before, defaultColor, font, fontSize, offsetX, startY);
                    offsetX += SplashKit.TextWidth(before, font, fontSize);

                    _window.FillRectangle(SplashKitSDK.Color.Gold, offsetX, startY, SplashKit.TextWidth(match, font, fontSize), SplashKit.TextHeight(match, font, fontSize));
                    _window.DrawText(match, defaultColor, font, fontSize, offsetX, startY);
                    offsetX += SplashKit.TextWidth(match, font, fontSize);

                    _window.DrawText(after, defaultColor, font, fontSize, offsetX, startY);
                    return;
                }
            }

            // No highlight
            _window.DrawText(line, defaultColor, font, fontSize, startX, startY);
        }
    }
}


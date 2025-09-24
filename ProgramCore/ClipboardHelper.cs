using System;
using TextCopy;

namespace PasswordManager
{
    public static class ClipboardHelper
    {
        public static void CopyToClipboard(string text)
        {
            try
            {
                ClipboardService.SetText(text);
            }
            catch (Exception ex)
            {
                // If clipboard access fails, fallback
                Console.WriteLine($"Failed to copy to clipboard: {ex.Message}");
                Console.WriteLine("Password: " + text);
            }
        }
    }
}

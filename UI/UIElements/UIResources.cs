using SplashKitSDK;
using System;

namespace PasswordManager
{
    public static class UIResources
    {
        public static Font Arial { get; private set; } = null!;
        public static Font ArialB { get; private set; } = null!;
        public static Bitmap Logo { get; private set; } = null!;

        public static void LoadResources()
        {
            Arial = SplashKit.LoadFont("ArialReg", "Resources/fonts/arial.ttf");
            ArialB = SplashKit.LoadFont("ArialBold", "Resources/fonts/arialbd.ttf");
            Logo = SplashKit.LoadBitmap("logo", "Resources/images/logo.png");
        }
    }
}



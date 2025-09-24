using SplashKitSDK;

namespace PasswordManager
{
    public static class UIConfig
    {
        //To save looking around for all the places to update UI elements, let's keep them all here...

        // --- Text Colours ---
        public static readonly SplashKitSDK.Color ButtonTextColour = SplashKitSDK.Color.MidnightBlue;
        public static readonly SplashKitSDK.Color ButtonEntryColour = SplashKitSDK.Color.DimGray;
        public static readonly SplashKitSDK.Color TextFontColour = SplashKitSDK.Color.DimGray;
        public static readonly SplashKitSDK.Color LabelFontColour = SplashKitSDK.Color.DimGray;
        public static readonly SplashKitSDK.Color HeaderFontColour = SplashKitSDK.Color.MidnightBlue;
        

        // --- Outline Colours ---
        public static readonly SplashKitSDK.Color ActionButtonBorderColour = SplashKitSDK.Color.DimGray;
        public static readonly SplashKitSDK.Color TextBorderColour = SplashKitSDK.Color.DimGray;

        // ---Text Box base/active colours ---
        public static readonly SplashKitSDK.Color TextBaseColour = SplashKitSDK.Color.White;
        public static readonly SplashKitSDK.Color TextFocusColour = SplashKitSDK.Color.PaleGoldenrod;



        // --- Button base/hover colours ---
        public static readonly SplashKitSDK.Color ButtonBaseColour = SplashKitSDK.Color.SandyBrown;
        public static readonly SplashKitSDK.Color ButtonHoverColour = SplashKitSDK.Color.DarkKhaki;
        public static readonly SplashKitSDK.Color ButtonWarningHoverColour = SplashKitSDK.Color.OrangeRed;


        // --- Fonts ---
        public static readonly Font DefaultFont = UIResources.Arial;
        public static readonly Font BoldFont = UIResources.Arial;
        public static readonly Font EntryButtonFont = UIResources.Arial;
        public static readonly Font ActionButtonFont = UIResources.Arial;

        // --- Font Sizes ---
        public static readonly int DefaultFontSize = 14;
        public static readonly int ActionButtonFontSize = 14;
        public static readonly int LabelFontSize = 10;
        public static readonly int HeaderFontSize = 16;

        // --- EntryList Button Sizes ---
        public const int EntryButtonHeight = 50;
        public const int EntryButtonSpacing = -1;
        public static int EntryButtonTotalHeight => EntryButtonHeight + EntryButtonSpacing;

        // --- ActionPanel Button Sizes ---
        public const int ActionButtonWidth = 60;
        public const int ActionButtonHeight = 25;
        public const int ActionButtonSpacing = 10;

        // --- RoundedButton Corner Radius ---
        public const int DefaultButtonRadius = 4;
    }
}

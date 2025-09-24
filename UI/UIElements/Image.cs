using SplashKitSDK;

namespace PasswordManager
{
    public class ImageElement : UIElement
    {
        private readonly Bitmap _bitmap;

        public ImageElement(Window window, int x, int y, Bitmap bitmap)
            : base(window, x, y)
        {
            _bitmap = bitmap;
        }

        public override void Draw()
        {
            _window.DrawBitmap(_bitmap, X, Y);
        }

        public override void HandleInput()
        {
            // Logo doesn’t need input
        }
    }
}

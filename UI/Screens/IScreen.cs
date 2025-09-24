using SplashKitSDK;

namespace PasswordManager
{

    public interface IScreen
    {
        void Draw();
        void HandleInput();
        bool IsComplete { get; set; }

    }
}

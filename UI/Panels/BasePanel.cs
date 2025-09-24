using System.Collections.Generic;
using SplashKitSDK;

namespace PasswordManager
{
    public abstract class Panel : UIElement
    {
        protected readonly List<UIElement> _children = new();

        public Panel(Window window, int x = 0, int y = 0, int width = 0, int height = 0)
            : base(window, x, y, width, height) { }

        public override void Draw()
        {
            foreach (var child in _children)
                child.Draw();
        }

        public override void HandleInput()
        {
            foreach (var child in _children)
                child.HandleInput();
        }
        
        public virtual IEnumerable<UIElement> GetElements()
        {
            foreach (var child in _children)
                yield return child;
        }
    }
}

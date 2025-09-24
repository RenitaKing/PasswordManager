using System.Collections.Generic;
using SplashKitSDK;

namespace PasswordManager
{
    public abstract class ScreenBase : IScreen
    {
        protected readonly Window _window;
        protected readonly List<UIElement> _uiElements = new();
        protected readonly List<UIElement> _focusableElements = new();
        protected int _focusIndex = 0;

        public virtual bool IsComplete { get; set; } = false;
        protected ScreenBase(Window window)
        {
            _window = window;
        }

        public abstract void Draw();
        // Centralized input handling for all elements
        public virtual void HandleInput()
        {
            HandleFocusInput();
            HandleInputForElements();
        }
        // Handles tabbing and mouse focus
        protected void HandleFocusInput()
        {
            if (_focusableElements.Count == 0) return;

            // Tab key navigation
            if (SplashKit.KeyTyped(KeyCode.TabKey))
            {
                bool shift = SplashKit.KeyDown(KeyCode.LeftShiftKey) || SplashKit.KeyDown(KeyCode.RightShiftKey);
                _focusableElements[_focusIndex].HasFocus = false;

                _focusIndex = shift
                    ? (_focusIndex - 1 + _focusableElements.Count) % _focusableElements.Count
                    : (_focusIndex + 1) % _focusableElements.Count;

                _focusableElements[_focusIndex].HasFocus = true;
            }

            // Mouse click focus
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                var mousePos = SplashKit.MousePosition();
                bool found = false;

                for (int i = 0; i < _focusableElements.Count; i++)
                {
                    var elem = _focusableElements[i];
                    var bounds = new SplashKitSDK.Rectangle() { X = elem.X, Y = elem.Y, Width = elem.Width, Height = elem.Height };

                    if (SplashKit.PointInRectangle(mousePos, bounds))
                    {
                        _focusableElements[_focusIndex].HasFocus = false;
                        _focusIndex = i;
                        elem.HasFocus = true;
                        found = true;
                        break;
                    }
                }

                if (!found && _focusableElements.Count > 0)
                {
                    _focusableElements[_focusIndex].HasFocus = false;
                }
            }
        }

        // Loops over all UIElements and calls HandleInput
        protected void HandleInputForElements()
        {
            foreach (var element in _uiElements)
            {
                element.HandleInput();
            }
        }
        //We'll use this if we need to refresh element after adding/subtracting
        protected void RefreshFocusableElements()
        {
            _focusableElements.Clear();
            foreach (var e in _uiElements)
            {
                if (e.CanFocus) _focusableElements.Add(e);
            }

            // Ensure at least one element has focus
            if (_focusableElements.Count > 0)
            {
                foreach (var e in _focusableElements) e.HasFocus = false;
                _focusIndex = 0;
                _focusableElements[0].HasFocus = true;
            }
        }

    }
}

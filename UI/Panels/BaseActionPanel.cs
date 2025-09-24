using SplashKitSDK;

namespace PasswordManager
{
    public abstract class ActionPanelBase<TAction> : Panel where TAction : Enum
    {
        protected readonly Dictionary<TAction, Button> _buttons = new();
        public event Action<TAction>? OnButtonClick;

        protected ActionPanelBase(
            Window window,
            int x,
            int y,
            int panelWidth,
            int panelHeight,
            int buttonHeight,
            int spacing,
            bool vertical = false,
            bool useRounded = false,
            Dictionary<TAction, string>? labels = null)
            : base(window, x, y)
        {
            int count = Enum.GetValues(typeof(TAction)).Length;

            // Auto-calc button width
            int buttonWidth = vertical
                ? panelWidth
                : (panelWidth - (spacing * (count - 1))) / count;

            int currentX = x;
            int currentY = y;

            foreach (TAction type in Enum.GetValues(typeof(TAction)))
            {
                string caption = labels != null && labels.ContainsKey(type)
                    ? labels[type]
                    : type.ToString();

                Button btn = useRounded
                    ? new RoundedButton(window, currentX, currentY, buttonWidth, buttonHeight, caption)
                    : new Button(window, currentX, currentY, buttonWidth, buttonHeight, caption);

                btn.OnClick += (b) => OnButtonClick?.Invoke(type);
                _buttons[type] = btn;

                if (vertical)
                    currentY += buttonHeight + spacing;
                else
                    currentX += buttonWidth + spacing;
            }
        }

        public override void Draw()
        {
            foreach (var btn in _buttons.Values)
                btn.Draw();
        }

        public override void HandleInput()
        {
            foreach (var btn in _buttons.Values)
                btn.HandleInput();
        }

        public override IEnumerable<UIElement> GetElements() => _buttons.Values;

        public Button GetButton(TAction type) => _buttons[type];
    }
}

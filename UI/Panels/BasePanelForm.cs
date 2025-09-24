using SplashKitSDK;
using System.Collections.Generic;

namespace PasswordManager
{
    public class FormPanel : UIElement
    {
        protected readonly List<FormField> _fields = new();
        protected readonly List<UIElement> _extraElements = new();
        protected int _currentY;      // track vertical layout
        protected int _labelSpacing = 3; // label to textbox
        protected int _fieldSpacing = 7;           // distance to next field
        protected int _width = 100;
        protected HeaderField? _header;


        public FormPanel(Window window, int x, int y, int width, int spacing = 7)
            : base(window, x, y)
        {
            _fieldSpacing = spacing;
            _width = width;
            _currentY = y;
        }

        public void AddField(FormField field) => _fields.Add(field);
        public void AddElement(UIElement element) => _extraElements.Add(element);

        public override void Draw()
        {
            // Draw header first (if any)
            _header?.Draw();
            //Draw fields

            foreach (var field in _fields)
            {
                field.Label.Draw();
                field.InputBox.Draw();
            }
            //Draw extra elements - buttons, etc
            foreach (var element in _extraElements)
                element.Draw();
        }

        public override void HandleInput()
        {
            foreach (var field in _fields)
                field.InputBox.HandleInput();

            foreach (var element in _extraElements)
                element.HandleInput();
        }

        public virtual IEnumerable<UIElement> GetElements()
        {
            foreach (var field in _fields)
            {
                yield return field.Label;     // include label
                yield return field.InputBox;  // include text field
            }

            foreach (var element in _extraElements)
                yield return element;
        }

        public Dictionary<string, string> GetValues()
        {
            var values = new Dictionary<string, string>();
            foreach (var field in _fields)
                values[field.Label.Text] = field.InputBox.Text;
            return values;
        }

        public void SetValues(Dictionary<string, string> values)
        {
            foreach (var field in _fields)
            {
                if (values.ContainsKey(field.Label.Text))
                    field.InputBox.SetText(values[field.Label.Text]);
            }
        }

        public string GetValue(string label)
        {
            var field = _fields.Find(f => f.Label.Text == label);
            return field?.InputBox.Text ?? "";
        }

        public void SetValue(string label, string value)
        {
            var field = _fields.Find(f => f.Label.Text == label);
            if (field != null)
                field.InputBox.SetText(value);
        }

        public virtual void Clear()
        {
            foreach (var field in _fields)
                field.InputBox.SetText("");
        }

        //Add different types of elements:
        protected void SetHeader(string text, int xOffset = 0)
        {
            // Place header at the current top Y of the panel
            _header = new HeaderField(_window, X + xOffset, _currentY, text);

            // Advance Y below the header with some spacing
            _currentY += _header.FontSize + _fieldSpacing;
        }
        protected LabelField AddLabel(string text, int xOffset = 0, int? height = null)
        {
            var label = new LabelField(_window, X + xOffset, _currentY, text);

            // Use provided height if given, otherwise default to label's height
            int labelHeight = height ?? label.Height;

            AddElement(label);

            // Advance Y for next element
            _currentY += labelHeight + _fieldSpacing;

            return label;
        }


        protected void AddLabelledField(ref TextField box, string labelText, int xOffset = 0, int fieldHeight = 25, bool multiline = false, int fieldWidth = -1)
        {
            int width = fieldWidth > 0 ? fieldWidth : _width;

            // Label above the text field
            var label = new LabelField(_window, X + xOffset, _currentY, labelText);
            _currentY += label.Height + _labelSpacing;

            // Text box directly below label
            box = new TextField(_window, X + xOffset, _currentY, width, fieldHeight);
            box.AllowMultiline = multiline;

            AddField(new FormField(label, box));

            // Move Y down for next field
            _currentY += fieldHeight + _fieldSpacing;
        }

        protected Button AddButtonUnderField(
            TextField field,
            string buttonText,
            int xOffset = 0,
            int Width = -1,
            int height = 25,
            bool rounded = true)
        {
            // X aligns with field; Y is just below the field
            int width = Width > 0 ? Width : _width;
            int btnX = field.X+xOffset;
            int btnY = field.Y + field.Height + _labelSpacing;

            Button button = rounded
                ? new RoundedButton(_window, btnX, btnY, width, height, buttonText)
                : new Button(_window, btnX, btnY, width, height, buttonText);

            AddElement(button);

            // Advance currentY so the next field is below the button
            _currentY = btnY + height + _fieldSpacing;

            return button;
        }
        protected void AddMultiButtonUnderField(
                TextField field,
                (string caption, Action<Button> onClick)[] buttons,
                int xOffset = 0,
                int height = 25,
                bool rounded = true,
                int spacingBetweenButtons = 5)
        {
            if (buttons.Length == 0) return;

            // Calculate total width available and individual button widths
            int totalWidth = _width; // Use form panel width
            int buttonWidth = (totalWidth - (spacingBetweenButtons * (buttons.Length - 1))) / buttons.Length;

            int btnX = field.X + xOffset;
            int btnY = field.Y + field.Height + _labelSpacing;

            foreach (var (caption, onClick) in buttons)
            {
                Button btn = rounded
                    ? new RoundedButton(_window, btnX, btnY, buttonWidth, height, caption)
                    : new Button(_window, btnX, btnY, buttonWidth, height, caption);

                btn.OnClick += onClick;

                AddElement(btn);

                btnX += buttonWidth + spacingBetweenButtons; // Move X for next button
            }

            // Advance currentY so the next field is below the buttons
            _currentY = btnY + height + _fieldSpacing;
        }


        
       
    }
}

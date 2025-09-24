using SplashKitSDK;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PasswordManager
{
    public class EntryForm : FormPanel
    {
        private TextField _websiteBox = null!;
        private TextField _usernameBox = null!;
        private TextField _passwordBox = null!;
        private TextField _categoryBox = null!;
        private TextField _notesBox = null!;

        public EntryForm(Window window, int x, int y, int width, int height = 395, int spacing = 8)
            : base(window, x, y, width, spacing)
        {
            SetHeader("Password Record Details");

            // Add fields
            AddLabelledField(ref _websiteBox, "Website");
            AddLabelledField(ref _usernameBox, "Username");
            AddLabelledField(ref _passwordBox, "Password");

            // Generate + Copy buttons under Password box
            AddMultiButtonUnderField(
                _passwordBox,
                new (string, Action<Button>)[]
                {
                    ("Generate Password", b => {
                        string newPass = PasswordService.GenerateStrongPassword();
                        _passwordBox.SetText(newPass);
            
                    }),
                    ("Copy Password", async b => {
                        string pwd = _passwordBox.Text;
                        _passwordBox.SelectAll();
                        ClipboardHelper.CopyToClipboard(pwd);
                        await Task.Delay(200);  //put in a short delay so the highlighting during copy hangs around for a moment
                        _passwordBox.ClearSelection();
                
                    })
                },
                rounded: false
            );

            AddLabelledField(ref _categoryBox, "Category");
            AddLabelledField(ref _notesBox, "Notes", fieldHeight: height - (_currentY - y), multiline: true);
        }

        // Strongly-typed properties for convenience
        public string Website => _websiteBox.Text;
        public string Username => _usernameBox.Text;
        public string Password => _passwordBox.Text;
        public string Category => _categoryBox.Text;
        public string Notes => _notesBox.Text;

        public void SetValues(string website, string username, string password, string notes = "", string category = "")
        {
            _websiteBox.SetText(website);
            _usernameBox.SetText(username);
            _passwordBox.SetText(password);
            _categoryBox.SetText(category);
            _notesBox.SetText(notes);
        }

        public override void Clear()
        {
            _websiteBox.SetText("");
            _usernameBox.SetText("");
            _passwordBox.SetText("");
            _categoryBox.SetText("");
            _notesBox.SetText("");
        }
    }
}

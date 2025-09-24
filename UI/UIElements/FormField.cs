using SplashKitSDK;
using System.Collections.Generic;

namespace PasswordManager
{
    public class FormField
{
    public LabelField Label { get; }
    public TextField InputBox { get; }

    public FormField(LabelField label, TextField inputBox)
    {
        Label = label;
        InputBox = inputBox;
    }

    public void Draw()
    {
        Label.Draw();
        InputBox.Draw();
    }

    public void HandleInput()
    {
        InputBox.HandleInput();
    }
}


    
}

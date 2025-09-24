using SplashKitSDK;

namespace PasswordManager
{
    public enum ActionButtonType
    {
        Clear,
        Add,
        Update,
        Delete
    }
    public class ActionPanel : ActionPanelBase<ActionButtonType>
    {
        public ActionPanel(Window window, int x, int y, int totalWidth = 390)
            : base(window, x, y,
                   panelWidth: totalWidth,
                   panelHeight: UIConfig.ActionButtonHeight,
                   buttonHeight: UIConfig.ActionButtonHeight,
                   spacing: UIConfig.ActionButtonSpacing,
                   vertical: false,
                   useRounded: false)
        {
            // Configure special button behaviour for the Delete button
        var deleteBtn = GetButton(ActionButtonType.Delete);
        deleteBtn.NeedsWarning = true; // this will set its hover colour to the warning colour
        }
    }

}

using SplashKitSDK;
using System;
using System.Collections.Generic;

namespace PasswordManager
{
    public enum GlobalActionType
    {
        Quit,
        ChangeMaster
    }

    public class GlobalPanel : ActionPanelBase<GlobalActionType>
    {
        private static readonly Dictionary<GlobalActionType, string> _labels = new()
    {
        { GlobalActionType.ChangeMaster, "Change Master" },
        { GlobalActionType.Quit, "Quit" }
    };

        public GlobalPanel(Window window, int x, int y, int totalWidth = 110)
            : base(window, x, y,
                   panelWidth: totalWidth,
                   panelHeight: UIConfig.ActionButtonHeight,
                   buttonHeight: UIConfig.ActionButtonHeight,
                   spacing: 10,
                   vertical: true,
                   useRounded: false,
                   labels: _labels)
        {
        }
    }
}

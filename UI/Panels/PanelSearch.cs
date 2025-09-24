using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace PasswordManager
{
    public class SearchPanel : FormPanel
    {
        private TextField _websiteBox= null!;
        private TextField _categoryBox= null!;

        public Button ClearButton { get; }
        public event Action? OnCleared;

        public SearchPanel(Window window, int x, int y, int panelWidth = 150, int fieldHeight = 25)
            : base(window, x, y, panelWidth)
        {
            SetHeader("Search or Filter Saved Entries");
            // Add fields
            AddLabelledField(ref _websiteBox, "Search by keyword", fieldHeight: fieldHeight);
            AddLabelledField(ref _categoryBox, "Filter by category", fieldHeight: fieldHeight);

            // Clear button below Category
            ClearButton = AddButtonUnderField(_categoryBox, "Clear Search",rounded:false);
            ClearButton.OnClick += (btn) =>
            {
                Clear();
                OnCleared?.Invoke();
            };
        }

        public string Website => _websiteBox.Text;
        public string Category => _categoryBox.Text;

        public override void Clear()
        {
            _websiteBox.SetText("");
            _categoryBox.SetText("");
        }
    }
}

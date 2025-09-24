using SplashKitSDK;
using static SplashKitSDK.SplashKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordManager
{
    public class EntryList : Panel
    {
        private List<EntryButton> _buttons = new();
        private int _scrollOffset = 0;
        private PasswordStore _store;
        public EntryButton? SelectedButton { get; set; }

        public event Action<EntryButton>? OnSelectionChanged;

        public EntryList(Window window, PasswordStore store, int x, int y, int width, int height)
            : base(window, x, y, width, height)
        {
            _store = store;
            Refresh();
        }

        // Draw
        public override void Draw()
        {
            Rectangle clipRect = SplashKit.RectangleFrom(X, Y, Width, Height);
            SplashKit.SetClip(clipRect);

            foreach (var btn in _buttons)
            {
                if (btn.Y + btn.Height >= Y && btn.Y <= Y + Height)
                    btn.Draw();
            }
            SplashKit.ResetClip();
        }

        // Handle input
        public override void HandleInput()
        {
            HandleScrollInput();
            HandleButtonFocus();
        }
        
        // Refresh list
        public void Refresh(string? searchTerm = null, string? category = null)
        {
            int? prevSelectedId = SelectedButton?.Entry.Id;

            ClearButtons();

            var filteredEntries = GetFilteredEntries(searchTerm, category);

            CreateButtons(filteredEntries, searchTerm, prevSelectedId);

            if (SelectedButton == null && _buttons.Count > 0)
            {
                SelectButton(_buttons[0]);
            }
            ScrollSelectedIntoView();
        }
        

        // Select a button programmatically
        public void SelectButton(EntryButton button)
        {
            if (button == null) return;

            foreach (var b in _buttons)
                b.IsSelected = false;

            button.IsSelected = true;
            SelectedButton = button;

            OnSelectionChanged?.Invoke(button);
        }

        // Clear buttons and children
        private void ClearButtons()
        {
            _buttons.Clear();
            _children.Clear();
        }

        // Filter entries
        private IEnumerable<PasswordEntry> GetFilteredEntries(string? searchTerm, string? category)
        {
            IEnumerable<PasswordEntry> filtered = _store.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                filtered = filtered.Where(e => e.Website.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrWhiteSpace(category))
                filtered = filtered.Where(e => !string.IsNullOrWhiteSpace(e.Category) &&
                                                e.Category.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0);

            return filtered;
        }

        // Create buttons
        private void CreateButtons(IEnumerable<PasswordEntry> entries, string? searchTerm, int? prevSelectedId)
        {
            int yPos = Y - _scrollOffset;

            foreach (var e in entries)
            {
                var btn = new EntryButton(_window, X, yPos, Width, UIConfig.EntryButtonHeight, e);
                btn.HighlightTerm = searchTerm;

                btn.OnClick += (b) =>
                {
                    SelectButton(b);
                };

                _buttons.Add(btn);
                _children.Add(btn);
                yPos += UIConfig.EntryButtonTotalHeight;

                // Restore previous selection
                if (prevSelectedId.HasValue && e.Id == prevSelectedId.Value)
                {
                    btn.IsSelected = true;
                    SelectedButton = btn;
                }
            }
        }

        // Scroll selected button into view
        private void ScrollSelectedIntoView()
        {
            if (SelectedButton == null) return;

            if (SelectedButton.Y < Y)
                _scrollOffset -= Y - SelectedButton.Y;
            else if (SelectedButton.Y + SelectedButton.Height > Y + Height)
                _scrollOffset += SelectedButton.Y + SelectedButton.Height - (Y + Height);

            ClampScroll();
        }

        private void ClampScroll()
        {
            int maxOffset = Math.Max(0, _buttons.Count * UIConfig.EntryButtonHeight - Height);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, maxOffset);
        }

        

        private void HandleScrollInput()
        {
            Vector2D scroll = SplashKit.MouseWheelScroll();
            _scrollOffset = (int)Math.Max(0, _scrollOffset - scroll.Y * UIConfig.EntryButtonHeight);
            ClampScroll();
        }

        private void HandleButtonFocus()
        {
            int yPos = Y - _scrollOffset;

            foreach (var btn in _buttons)
            {
                btn.Y = yPos;
                yPos += UIConfig.EntryButtonTotalHeight;

                btn.HandleInput();

                if (btn.HasFocus)
                {
                    SelectButton(btn);
                }
            }
        }

 
    }
}

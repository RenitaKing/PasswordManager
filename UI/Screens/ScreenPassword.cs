using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace PasswordManager
{

    public class PasswordScreen : ScreenBase
    {
        private readonly PasswordStore _store;
        private readonly EntryForm _entryForm;
        private readonly EntryList _entryList;
        private readonly ActionPanel _actionPanel;
        private readonly SearchPanel _searchPanel;
        private readonly GlobalPanel _globalPanel;
        private readonly EntryListHeader _headerPanel;
        private bool _refreshNeeded = false;

        public bool QuitRequested { get; private set; } = false;
        public bool ChangeMasterPasswordRequested { get; set; } = false;

        public PasswordScreen(Window window, PasswordStore store) : base(window)
        {
            _store = store;
            //Initialise all our panels
            _globalPanel = new GlobalPanel(window, window.Width - 120, 5);
            _searchPanel = new SearchPanel(window, 410, 5, 200);
            _entryForm = new EntryForm(window, 410, 170, 380, 370);
            _entryList = new EntryList(window, _store, 10, 35, 380, 555);
            _headerPanel = new EntryListHeader(window, _entryList.X, 5,_entryList.Width,30,"Saved Entries (use mouse-wheel to scroll)");
            _actionPanel = new ActionPanel(window, 410, 560,380);

            // Subscribe to panel events
            SetupPanelEvents();

            // Aggregate UI elements from panels
            RefreshUIElements();
        }

        private void SetupPanelEvents()
        {
            // --- Search Panel ---
            _searchPanel.OnCleared += () => _refreshNeeded = true;

            // --- Global Panel ---
            _globalPanel.OnButtonClick += type =>
            {
                switch (type)
                {
                    case GlobalActionType.ChangeMaster:
                        ChangeMasterPasswordRequested = true;
                        IsComplete = true;
                        break;
                    case GlobalActionType.Quit:
                        QuitRequested = true;
                        IsComplete = true;
                        break;
                }
            };

            // --- Entry List ---
            _entryList.OnSelectionChanged += btn =>
            {
                _entryForm.SetValues(btn.Entry.Website, btn.Entry.Username, btn.Entry.Password,
                                     btn.Entry.Notes, btn.Entry.Category);
            };

            // --- Action Panel ---
            _actionPanel.OnButtonClick += type =>
            {
                _refreshNeeded = false;
                switch (type)
                {
                    case ActionButtonType.Add:
                        if (AddEntry()) _refreshNeeded = true;
                        break;
                    case ActionButtonType.Update:
                        if (UpdateEntry()) _refreshNeeded = true;
                        break;
                    case ActionButtonType.Delete:
                        if (DeleteEntry()) _refreshNeeded = true;
                        break;
                    case ActionButtonType.Clear:
                        _entryForm.Clear();
                        break;
                }
            };
        }

        public override void Draw()
        {
            // Draw all our panels

            _window.DrawRectangle(SplashKitSDK.Color.DarkKhaki, 10, 35, 380, 555); //Password list zone
            //_window.FillRectangle(Color.SeaShell, 0, 50, 400, 550); //Password list
            //_window.FillRectangle(Color.WhiteSmoke, 400, 0, 400, 150);//Search bar
            //_window.FillRectangle(Color.WhiteSmoke, 400, 150, 400, 400);//Details
            //_window.FillRectangle(Color.DarkGray, 400, 550, 400, 50);//Action Bar

            _globalPanel.Draw();
            _searchPanel.Draw();
            _entryList.Draw();
            _headerPanel.Draw();
            _entryForm.Draw();
            _actionPanel.Draw();
        }

        public override void HandleInput()
        {
            HandleFocusInput();
            // Handle EntryList scroll directly, since it needs to detect the mousewheel
            _entryList.HandleInput();

            // Delegate input to other UI elements
            foreach (var el in _uiElements)
            {
                // Skip _entryList now, since it's already done
                if (el != _entryList)
                    el.HandleInput();
            }

            //Refresh our entry list if needed
            if (_refreshNeeded || SplashKit.KeyTyped(KeyCode.ReturnKey))
            {
                RefreshEntryList();
                _refreshNeeded = false;
            }
        }


        #region Entry Actions
        private bool AddEntry()
        {
            if (!string.IsNullOrWhiteSpace(_entryForm.Website) &&
                !string.IsNullOrWhiteSpace(_entryForm.Username) &&
                !string.IsNullOrWhiteSpace(_entryForm.Password))
            {
                _store.AddEntry(_entryForm.Website, _entryForm.Username, _entryForm.Password, _entryForm.Notes, _entryForm.Category);
                return true;
            }
            return false;
        }

        private bool UpdateEntry()
        {
            if (_entryList.SelectedButton != null)
            {
                var e = _entryList.SelectedButton.Entry;
                e.Website = _entryForm.Website;
                e.Username = _entryForm.Username;
                e.Password = _entryForm.Password;
                e.Notes = _entryForm.Notes;
                e.Category = _entryForm.Category;
                return true;
            }
            return false;
        }

        private bool DeleteEntry()
        {
            if (_entryList.SelectedButton != null)
            {
                _store.DeleteById(_entryList.SelectedButton.Entry.Id);
                _entryList.SelectedButton = null;
                _entryForm.Clear();
                return true;
            }
            return false;
        }
        #endregion

        #region UI Updates
        private void RefreshEntryList()
        {
            int? prevSelectedId = _entryList.SelectedButton?.Entry.Id;

            _entryList.Refresh(_searchPanel.Website, _searchPanel.Category);

            EntryButton? buttonToSelect = null;

            // Restore selection if possible
            if (prevSelectedId.HasValue)
            {
                buttonToSelect = _entryList.GetElements()
                                    .OfType<EntryButton>()
                                    .FirstOrDefault(b => b.Entry.Id == prevSelectedId.Value);
            }
            if (buttonToSelect == null)
                buttonToSelect = _entryList.GetElements().OfType<EntryButton>().FirstOrDefault();

            if (buttonToSelect != null)
                _entryList.SelectButton(buttonToSelect);

            RefreshUIElements(); // rebuild focusable list
            _refreshNeeded = false;

        }

        private void RefreshUIElements()
        {

            _uiElements.Clear();
            //Add panels and their buttons
            _uiElements.AddRange(_actionPanel.GetElements());
            _uiElements.AddRange(_globalPanel.GetElements());
            _uiElements.AddRange(_entryForm.GetElements());
            _uiElements.AddRange(_entryList.GetElements());
            _uiElements.AddRange(_searchPanel.GetElements());


            //Build focusable list to tab through
            _focusableElements.Clear();
            foreach (var el in _uiElements)
            {
                if (el.CanFocus) _focusableElements.Add(el);
            }

            // Reset _focusIndex safely
            _focusIndex = _focusableElements.Count > 0 ? 0 : -1;

            // Ensure at least one element has focus
            if (_focusIndex > 0)
                _focusableElements[_focusIndex].HasFocus = true;
        }
        #endregion
    }
}

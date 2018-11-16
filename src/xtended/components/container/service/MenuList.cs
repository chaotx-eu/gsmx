namespace GSMXtended {
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;

    /// A menu which is capable of holdin several
    /// control items and offering the ability to
    /// select beetwen them
    public class MenuList : Container {
        /// Event handler for when an item is selected
        public event SelectedHandler SelectedEvent;

        /// Event handler for when an item is deselected
        public event SelectedHandler DeselectedEvent;

        /// Event handler for when any of the ActionKeys or
        /// ActionButtons is pressed
        public event SelectedHandler ActionEvent;

        /// Event handler for when any of the CancelKeys or
        /// CancelButtons is pressed
        public event EventHandler CancelEvent;

        /// List of keyboard keys which can trigger an action event
        public List<Keys> ActionKeys {get; set;}

        /// List of gamepad buttons which can trigger an action event
        public List<Buttons> ActionButtons {get; set;}

        /// List of keyboard keys which can trigger a cancel event
        public List<Keys> CancelKeys {get; set;}

        /// List of gamepad buttons which can trigger a cancel event
        public List<Buttons> CancelButtons {get; set;}

        /// Currently selected item in this menu
        private MenuItem selected, lastSelected;
        public Control SelectedItem {get {return selected;}}

        /// Color of the selected item
        public Color SelectedColor {get; set;} = Color.Yellow;

        /// Wether this list is currently focused, when this Menu gains
        /// focus and it was not focused before the InputTimer resets
        private bool isFocused;
        public bool IsFocused {
            get {return isFocused;}
            set {
                if(value && !isFocused) InputTimer = 0;
                isFocused = value;
            }
        }

        /// Wether this list is static i.e. all items
        /// are schown and the cursor moves betweem them
        /// or dynamic where only the selected item is
        /// shown in the center (plus any number of items
        /// defined in VisibleRange fading away)
        public bool IsStatic {get; set;} = false;

        /// The time past in milliseconds since the last valid input
        public int InputTimer {get; set;}

        /// Wether the user input is locked for this list,
        /// setting this to false will reset the input timer
        public bool InputLocked {
            get {return InputTimer < MillisPerInput;}
        }

        /// The number of items visible next to the selected
        /// item if this Menu is dynamic (i.e. IsStatic == false)
        public int VisibleRange {get; set;} = 1;

        /// How many milliseconds will pass until the
        /// next user input is going to be queued
        public int MillisPerInput {get; set;} = 144;

        /// The index of the selected item, it can
        /// be used to select a specific item
        private int selectedIndex, lastSelectedIndex;
        public int SelectedIndex {
            get {return selectedIndex;}
            set {
                try {
                    selectedIndex = value;
                    lastSelected = selected;
                    selected = (MenuItem)Children[value];
                    selected.IsSelected = true;
                    onSelected(new SelectedEventArgs(selectedIndex, selected));
                } catch(System.ArgumentOutOfRangeException) {
                    selected = null;
                }

                if(lastSelected != null && lastSelected != selected) {
                    lastSelected.IsSelected = false;
                    onDeselected(new SelectedEventArgs(lastSelectedIndex, lastSelected));
                }
            }
        }


        public MenuList(params Control[] items) : base(items) {
            // size of this pane depends on its children
            PercentWidth = PercentHeight = -1;

            // default action key/s
            ActionKeys = new List<Keys>(new Keys[]{Keys.Enter});

            // default action button/s
            ActionButtons = new List<Buttons>(new Buttons[]{Buttons.A});

            // default cancel key/s
            CancelKeys = new List<Keys>(new Keys[]{Keys.Back});

            // default cancel button/s
            CancelButtons = new List<Buttons>(new Buttons[]{Buttons.B});

            // action event handler
            KeyPressedEvent += (sender, args) => {
                if(!IsFocused || InputLocked) return;

                // TODO -> DRY!
                foreach(Keys key in ActionKeys) {
                    if(args.KeyboardState.Value.IsKeyDown(key)) {
                        onAction(new SelectedEventArgs(SelectedIndex, SelectedItem));
                        InputTimer = 0;
                        return;
                    }
                }

                foreach(Buttons button in ActionButtons) {
                    if(args.GamePadState.Value.IsButtonDown(button)) {
                        onAction(new SelectedEventArgs(SelectedIndex, SelectedItem));
                        InputTimer = 0;
                        return;
                    }
                }

                foreach(Keys key in CancelKeys) {
                    if(args.KeyboardState.Value.IsKeyDown(key)) {
                        onCancel(null);
                        InputTimer = 0;
                        return;
                    }
                }

                foreach(Buttons button in CancelButtons) {
                    if(args.GamePadState.Value.IsButtonDown(button)) {
                        onCancel(null);
                        InputTimer = 0;
                        return;
                    }
                }
            };
        }

        /// Selects the item at the specific index,
        /// if index is negative the value will be
        /// subtracted from the current selected index
        public void select(int index, bool circular) {
            SelectedIndex = index < 0
                ? (circular
                    ? (SelectedIndex + Children.Count - ((-index)%Children.Count))%Children.Count
                    : Math.Max(0, SelectedIndex + index))
                : (circular
                    ? index%Children.Count
                    : Math.Min(index, Children.Count-1));
        }

        /// Overload of select(int, bool) for convenience,
        /// selects the item at index with circular disabled
        public void select(int index) {
            select(index, false);
        }

        /// Overload of select(int, bool) for convenience,
        /// selects the next item with circular as argument
        public void select(bool circular) {
            select(SelectedIndex+1, circular);
        }

        /// Overload of select(int) for convenience,
        /// selects the next item with circular disabled
        public void select() {
            select(SelectedIndex+1);
        }

        /// Updates the inputTimer and locks/unlocks user input
        public override void update(GameTime time) {
            if(InputTimer < MillisPerInput)
                InputTimer += time.ElapsedGameTime.Milliseconds;

            base.update(time);
        }

        /// Event handler for when an item is selected
        protected virtual void onSelected(SelectedEventArgs args) {
            SelectedHandler handler = SelectedEvent;
            if(handler != null) handler(this, args);
        }

        /// Event handler for when an item is deselected
        protected virtual void onDeselected(SelectedEventArgs args) {
            SelectedHandler handler = DeselectedEvent;
            if(handler != null) handler(this, args);
        }

        /// Event handler for when any action button/key is pressed
        protected virtual void onAction(SelectedEventArgs args) {
            SelectedHandler handler = ActionEvent;
            if(handler != null) handler(this, args);
        }

        /// Event handler for when any cancel button/key is pressed
        protected virtual void onCancel(EventArgs args) {
            EventHandler handler = CancelEvent;
            if(handler != null) handler(this, args);
        }
    }
}
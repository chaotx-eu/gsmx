namespace GSMXtended {
    using System;
    using System.Linq;
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

        /// List of keyboard keys with which the next item is selected
        public List<Keys> ControlNextKeys {get; set;}

        /// List of gamepad buttons with which the next item is selected
        public List<Buttons> ControlNextButtons {get; set;}

        /// List of keyboard keys with which the previous item is selected
        public List<Keys> ControlPreviousKeys {get; set;}
        
        /// List of gamepad buttons with which the previous item is selected
        public List<Buttons> ControlPreviousButtons {get; set;}

        /// Currently selected item in this menu
        private ScreenComponent selected, lastSelected;
        public ScreenComponent SelectedItem {get {return selected;}}

        /// Color of the selected item
        public Color SelectedColor {get; set;} = Color.Yellow;

        /// Wether this list is currently focused, when this Menu gains
        /// focus and it was not focused before the InputTimer resets
        private bool isFocused;
        public bool IsFocused {
            get {
                bool parentFocused = true;
                Container parent = ParentContainer;
                while(parent != null) {
                    if(parent is MenuList) {
                        parentFocused = ((MenuList)parent).IsFocused;
                        break;
                    }

                    parent = parent.ParentContainer;
                }

                return isFocused && parentFocused;
            }

            set {
                if(value && !isFocused) InputTimer = 0;
                isFocused = value;
            }
        }

        protected void applyFocus(ScreenComponent child, bool focus) {
            if(child is MenuList)
                ((MenuList)child).IsFocused = focus;

            if(child is Container)
                ((Container)child).Children.ToList().ForEach(child2
                    => applyFocus(child2, focus));
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

        /// If true a pressed key/button has to be released first
        /// before it can trigger an key pressed event again
        public bool InputSingleMode {get; set;}

        /// The number of items visible next to the selected
        /// item if this Menu is dynamic (i.e. IsStatic == false)
        public int VisibleRange {get; set;} = 1;

        /// How many milliseconds have to pass until the
        /// next user input can be queued
        private int millisPerInput = 240, trueMillisPerInput;
        public int MillisPerInput {
            get {return trueMillisPerInput;}
            set {millisPerInput = value;}
        }

        /// Minimum milliseconds between inputs
        public int MinMillisPerInput {get; set;} = 112;

        /// Deaccaleration of MilliesPerInput within a second
        /// which starts when a key is pressed and not released
        /// during the next queue
        public int MillisPerInputDeacceleration {get; set;} = 128;

        private List<Keys> lastKeys = new List<Keys>();
        private List<Buttons> lastButtons = new List<Buttons>();

        /// The index of the selected item, it can
        /// be used to select a specific item
        private int selectedIndex, lastSelectedIndex;
        public int SelectedIndex {
            get {return selectedIndex;}
            set {
                try {
                    lastSelectedIndex = selectedIndex;
                    lastSelected = selected;
                    selectedIndex = value;
                    selected = Children[value];

                    if(selected is MenuItem) ((MenuItem)selected).IsSelected = true;
                    if(selected is MenuList) ((MenuList)selected).IsFocused = true;
                    onSelected(new SelectedEventArgs(selectedIndex, selected));
                } catch(System.ArgumentOutOfRangeException) {
                    selected = null;
                }

                if(lastSelected != null && lastSelected != selected) {
                    if(lastSelected is MenuItem) ((MenuItem)lastSelected).IsSelected = false;
                    if(lastSelected is MenuList) ((MenuList)lastSelected).IsFocused = false;
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
                foreach(Keys key in ControlNextKeys) {
                    if(args.KeyboardState.Value.IsKeyDown(key)) {
                        if(lastKeys.Contains(key)) {
                            if(InputSingleMode) return;
                        } else lastKeys.Add(key);
                        InputTimer = 0;
                        select();
                    } else lastKeys.Remove(key);
                }

                foreach(Buttons button in ControlNextButtons) {
                    if(args.GamePadState.Value.IsButtonDown(button)) {
                        if(lastButtons.Contains(button)) {
                            if(InputSingleMode) return;
                        } else lastButtons.Add(button);
                        InputTimer = 0;
                        select();
                    } else lastButtons.Remove(button);
                }

                foreach(Keys key in ControlPreviousKeys) {
                    if(args.KeyboardState.Value.IsKeyDown(key)) {
                        if(lastKeys.Contains(key)) {
                            if(InputSingleMode) return;
                        } else lastKeys.Add(key);
                        InputTimer = 0;
                        select(-1);
                    } else lastKeys.Remove(key);
                }

                foreach(Buttons button in ControlPreviousButtons) {
                    if(args.GamePadState.Value.IsButtonDown(button)) {
                        if(lastButtons.Contains(button)) {
                            if(InputSingleMode) return;
                        } else lastButtons.Add(button);
                        InputTimer = 0;
                        select(-1);
                    } else lastButtons.Remove(button);
                }

                foreach(Keys key in ActionKeys) {
                    if(args.KeyboardState.Value.IsKeyDown(key)) {
                        if(lastKeys.Contains(key)) {
                            if(InputSingleMode) return;
                        } else lastKeys.Add(key);

                        onAction(new SelectedEventArgs(SelectedIndex, SelectedItem));
                        InputTimer = 0;
                        return;
                    } else lastKeys.Remove(key);
                }

                foreach(Buttons button in ActionButtons) {
                    if(args.GamePadState.Value.IsButtonDown(button)) {
                        if(lastButtons.Contains(button)) {
                            if(InputSingleMode) return;
                        } else lastButtons.Add(button);
                        
                        onAction(new SelectedEventArgs(SelectedIndex, SelectedItem));
                        InputTimer = 0;
                        return;
                    } else lastButtons.Remove(button);
                }

                foreach(Keys key in CancelKeys) {
                    if(args.KeyboardState.Value.IsKeyDown(key)) {
                        if(lastKeys.Contains(key)) {
                            if(InputSingleMode) return;
                        } else lastKeys.Add(key);

                        onCancel(null);
                        InputTimer = 0;
                        return;
                    } else lastKeys.Remove(key);
                }

                foreach(Buttons button in CancelButtons) {
                    if(args.GamePadState.Value.IsButtonDown(button)) {
                        if(lastButtons.Contains(button)) {
                            if(InputSingleMode) return;
                        } else lastButtons.Add(button);

                        onCancel(null);
                        InputTimer = 0;
                        return;
                    } else lastButtons.Remove(button);
                }
            };
        }

        public override void add(params ScreenComponent[] items) {
            base.add(items);
            select(selectedIndex);
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
            int elapsed = time.ElapsedGameTime.Milliseconds;
            if(InputTimer < MillisPerInput)
                InputTimer += elapsed;

            // deaccelerate while any key/button is pressed
            if(lastKeys.Count > 0 || lastButtons.Count > 0) {
                trueMillisPerInput = (int)Math.Max(MinMillisPerInput,
                    trueMillisPerInput - MillisPerInputDeacceleration*(elapsed/1000f));
            } else trueMillisPerInput = millisPerInput;

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
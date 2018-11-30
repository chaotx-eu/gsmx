namespace GSMXtended {
    using System;
    
    /// Event arguments passed to selected item events
    public class SelectedEventArgs : EventArgs {
        public int SelectedIndex {get;}
        public ScreenComponent SelectedItem {get;}

        public SelectedEventArgs(int selectedIndex, ScreenComponent selectedItem) {
            SelectedIndex = selectedIndex;
            SelectedItem = selectedItem;
        }
    }

    /// Event handler template for selected item events
    public delegate void SelectedHandler(MenuList sender, SelectedEventArgs args);
}
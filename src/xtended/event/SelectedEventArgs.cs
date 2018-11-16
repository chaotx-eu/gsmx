namespace GSMXtended {
    using System;
    
    /// Event arguments passed to selected item events
    public class SelectedEventArgs : EventArgs {
        public int SelectedIndex {get;}
        public Control SelectedItem {get;}

        public SelectedEventArgs(int selectedIndex, Control selectedItem) {
            SelectedIndex = selectedIndex;
            SelectedItem = selectedItem;
        }
    }

    /// Event handler template for selected item events
    public delegate void SelectedHandler(MenuList sender, SelectedEventArgs args);
}
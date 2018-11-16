namespace GSMXtended {
    using System;
    using Microsoft.Xna.Framework.Input;

    /// Event args to be passed when a key or button
    /// is pressed on a selected/active screen component
    public class KeyEventArgs : EventArgs {
        public KeyboardState? KeyboardState {get;}
        public GamePadState? GamePadState {get;}
        public int DeviceIndex {get;}

        public KeyEventArgs(KeyboardState? keyboardState,
        GamePadState? gamePadState, int deviceIndex) {
            KeyboardState = keyboardState;
            GamePadState = gamePadState;
            DeviceIndex = deviceIndex;
        }
    }

    /// default key event handler structure
    public delegate void KeyEventHandler(ScreenComponent sender, KeyEventArgs args);
}
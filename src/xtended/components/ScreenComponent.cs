namespace GSMXtended {
    using System;
    using GameStateManagement;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    // public enum Alignment {Center, Top, Right, Bottom, Left}
    public enum HAlignment {Center, Right, Left}
    public enum VAlignment {Center, Top, Bottom}

    /// Base class of all components a screen may have
    public class ScreenComponent {
        /// The screen this component belongs to
        public virtual XtendedScreen ParentScreen {get; set;}

        /// The container this component belongs to
        public Container ParentContainer {get; set;}

        /// The base width in pixels of this component
        public virtual int Width {get; set;}

        /// The base height in pixels of this component
        public virtual int Height {get; set;}
        
        /// The percentage width of this component relative
        /// to its ParentContainers width or in case there
        /// is none to its ParentScreens width
        public int PercentWidth {get; set;}

        /// The percentage height of this component relative
        /// to its ParentContainers height or in case there
        /// is none to its ParentScreens height
        public int PercentHeight {get; set;}

        /// The x-coordinate of this component within its
        /// ParentContainer or in case there is none within
        /// its ParentScreen
        private float x, targetX;
        public virtual float X {
            get {return x;}
            set {targetX = value;}
        }

        /// The y-coordinate of this component within its
        /// ParentContainer or in case there is none within
        /// its ParentScreen
        private float y, targetY;
        public virtual float Y {
            get {return y;}
            set {targetY = value;}
        }

        /// The alpha value of this components color
        private float alpha, targetAlpha = 1f;
        public virtual float Alpha {
            get {return alpha*EffectAlpha;}
            set {targetAlpha = value;}
        }

        /// Alpha value used for effects and similiar
        /// (alpha will be multiplied with this value)
        private float effectAlpha, targetEffectAlpha = 1f;
        public virtual float EffectAlpha {
            get {return effectAlpha;}
            set {targetEffectAlpha = value;}
        }

        /// The color of this component
        private Color color;
        public virtual Color Color {
            get {return color*Alpha;}
            set {color = value;}
        }

        /// Target x-coordinate
        public float TargetX {get {return targetX;}}

        /// Target y-coordinate
        public float TargetY {get {return targetY;}}

        /// Target alpha value
        public float TargetAlpha {get {return targetAlpha;}}

        /// How many pixels per second this component can
        /// move, values below < 0 are equivalent to inifinite
        public int PixelPerSecond {get; set;} = 288;

        /// How many milliseconds it will take to fade
        /// the background alpha from 0 to 1
        public int MillisPerAlpha {get; set;} = 640;

        /// The horizontal alignment of this component
        /// if null the component will be centered
        public HAlignment HAlignment {get; set;}

        /// The vertical alignment of this component
        /// if null the component will be centered
        public VAlignment VAlignment {get; set;}

        /// Wether this component should calculate its position
        /// and size dynamically based of its Alignment, PercentWidth
        /// and -Height and the layout strategy of the ParentContainer
        public bool Managed {get; set;} = true; // TODO not really in use anymore

        /// Event handler which gets triggered when a key or button is
        /// pressed while this item is selected/active
        public event KeyEventHandler KeyPressedEvent;

        /// Number of immediate updates in which positioning
        /// (TODO sizing and scaling?) will be immediate
        protected int immediateUpdates = 3; // Three are at least required for
                                            // everything to align properly

        /// Loads the required ressources
        public virtual void load() {}

        /// Initializes component
        public virtual void init() {}

        /// The update method of this component which is meant to
        /// be called once every game tick
        public virtual void update() {update(null);} // deprecated
        public virtual void update(GameTime time) {
            float fragment;

            if(immediateUpdates <= 0 && PixelPerSecond > 0) {
                fragment = (float)(time.ElapsedGameTime.Milliseconds/1000f)*PixelPerSecond;
                if(x < targetX) x = Math.Min(targetX, x+fragment);
                if(x > targetX) x = Math.Max(targetX, x-fragment);
                if(y < targetY) y = Math.Min(targetY, y+fragment);
                if(y > targetY) y = Math.Max(targetY, y-fragment);
            } else if(x != targetX || y != targetY) {
                x = targetX;
                y = targetY;
                if(immediateUpdates > 0) --immediateUpdates;
            }

            if(MillisPerAlpha > 0) {
                fragment = (float)time.ElapsedGameTime.Milliseconds/MillisPerAlpha;
                if(alpha < targetAlpha) alpha = Math.Min(targetAlpha, alpha+fragment);
                if(alpha > targetAlpha) alpha = Math.Max(targetAlpha, alpha-fragment);
                if(effectAlpha < targetEffectAlpha) effectAlpha = Math.Min(targetEffectAlpha, effectAlpha+fragment);
                if(effectAlpha > targetEffectAlpha) effectAlpha = Math.Max(targetEffectAlpha, effectAlpha-fragment);
            } else {
                alpha = targetAlpha;
                effectAlpha = targetEffectAlpha;
            }
        }

        /// Draws this component to the screen
        public virtual void draw() {}

        /// Handles user input during update but only if the
        /// parent screen is active and focused
        public void handleInput(InputState inputState) {
            KeyboardState[] keyboardStates = inputState.CurrentKeyboardStates;
            GamePadState[] gamepadStates = inputState.CurrentGamePadStates;

            int kbsl = keyboardStates.Length;
            int gpsl = gamepadStates.Length;
            int max = Math.Max(kbsl, gpsl);

            for(int i = 0; i < max; ++i) {
                onKeyPressed(new KeyEventArgs(
                    i < kbsl ? keyboardStates[i] : (KeyboardState?)null,
                    i < gpsl ? gamepadStates[i] : (GamePadState?)null, i));
            }
            
            if(this is Container)
                foreach(ScreenComponent child in ((Container)this).Children)
                    child.handleInput(inputState);
        }

        /// Wrapper for key pressed event handler
        protected virtual void onKeyPressed(KeyEventArgs args) {
            KeyEventHandler handler = KeyPressedEvent;
            if(handler != null) handler(this, args);
        }
    }
}
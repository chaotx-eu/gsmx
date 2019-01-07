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
            get {return alpha*EffectAlpha*AlphaMod;}
            set {targetAlpha = value;}
        }

        /// Alpha value used for effects and similiar
        /// (alpha will be multiplied with this value)
        private float effectAlpha, targetEffectAlpha = 1f;
        public virtual float EffectAlpha {
            get {return effectAlpha;}
            set {targetEffectAlpha = value;}
        }

        /// Alpha value modifier which is multiplied
        /// with alpha and the alpha mod from the
        /// parent container/screen of this component
        private float alphaMod = 1f, targetAlphaMod = 1f;
        public float AlphaMod {
            get {
                return alphaMod*(ParentContainer != null
                    ? ParentContainer.AlphaMod : 1);
            }
            set {targetAlphaMod = value;}
        }

        /// The scaling of this component. Rather use default
        /// scale to scale this component
        private float scale, targetScale = 1f;
        public virtual float Scale {
            get {return scale;}
            set {targetScale = value;}
        }

        /// Scaling used for effects and similiar, size
        /// attributes may be mutliplied with this on demand
        private float effectScale, targetEffectScale = 1f;
        public virtual float EffectScale {
            get {return effectScale;}
            set {targetEffectScale = value;}
        }

        /// The default default scaling of this component. Use
        /// this to set the scale on this object in case it may
        /// be placed within a menu list
        private float defaultScale = 1;
        public float DefaultScale {
            get {return defaultScale;} 
            set {
                defaultScale = value;
                Scale = value;
            }
        }

        /// Readonly target scale value
        public virtual float TargetScale {get {return targetScale;}}

        /// Readonly target effect scale value
        public virtual float TargetEffectScale {get {return EffectScale;}}

        /// Readonly target alpha mod value
        public float TargetAlphaMod {get {return alphaMod;}}

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
        public float TargetAlpha {
            get {return targetAlpha
                *(ParentScreen != null
                ? ParentScreen.Alpha : 1);
            }
        }

        /// How many pixels per second this component can
        /// move, values below < 0 are equivalent to inifinite
        public int PixelPerSecond {
            get {return (int)(pixelPerSecond*PPSFactor);}
            set {pixelPerSecond = value;}
        }

        /// How many milliseconds it will take to fade
        /// the background alpha from 0 to 1
        public int MillisPerAlpha {
            get {return (int)(millisPerAlpha*MPAFactor);}
            set {millisPerAlpha = value;}
        }

        /// How many milliseconds it would take to scale
        /// this item from 0 to 1, values below 0 are
        /// equivalent to 0 (for now this will also
        /// affect the alpha value)
        public int MillisPerScale {
            get {return (int)(millisPerScale*MPSFactor);}
            set {millisPerScale = value;}
        }

        /// Factor PixelPerSecond is multiplied with
        public float PPSFactor {
            get {return ppsFactor*(ParentContainer != null
                ? ParentContainer.PPSFactor : 1);}
            set {ppsFactor = value;}
        }

        /// Factor MillisPerAlpha is multiplied with
        public float MPAFactor {
            get {return mpaFactor*(ParentContainer != null
                ? ParentContainer.MPAFactor : 1);}
            set {mpaFactor = value;}
        }

        /// Factor MillisPerScale is multiplied with
        public float MPSFactor {
            get {return mpsFactor*(ParentContainer != null
                ? ParentContainer.MPSFactor : 1);}
            set {mpsFactor = value;}
        }

        /// The horizontal alignment of this component
        /// if null the component will be centered
        public HAlignment HAlignment {get; set;}

        /// The vertical alignment of this component
        /// if null the component will be centered
        public VAlignment VAlignment {get; set;}

        /// Event handler which gets triggered when a key or button is
        /// pressed while this item is selected/active
        public event KeyEventHandler KeyPressedEvent;

        private int pixelPerSecond = 288;
        private int millisPerAlpha = 640;
        private int millisPerScale = 456;
        private float ppsFactor = 1;
        private float mpaFactor = 1;
        private float mpsFactor = 1;

        /// Loads the required ressources
        public virtual void load() {}

        /// Initializes component
        public virtual void init() {}

        /// The update method of this component which is meant to
        /// be called once every game tick
        public virtual void update() {update(null);} // deprecated
        public virtual void update(GameTime time) {
            float fragment;

            if(PixelPerSecond > 0) {
                fragment = (float)(time.ElapsedGameTime.Milliseconds/1000f)*PixelPerSecond;
                if(x < targetX) x = Math.Min(targetX, x+fragment);
                if(x > targetX) x = Math.Max(targetX, x-fragment);
                if(y < targetY) y = Math.Min(targetY, y+fragment);
                if(y > targetY) y = Math.Max(targetY, y-fragment);
            } else if(x != targetX || y != targetY) {
                x = targetX;
                y = targetY;
            }

            if(MillisPerAlpha > 0) {
                fragment = (float)time.ElapsedGameTime.Milliseconds/MillisPerAlpha;
                if(alpha < TargetAlpha) alpha = Math.Min(TargetAlpha, alpha+fragment);
                if(alpha > TargetAlpha) alpha = Math.Max(TargetAlpha, alpha-fragment);
                if(effectAlpha < targetEffectAlpha) effectAlpha = Math.Min(targetEffectAlpha, effectAlpha+fragment);
                if(effectAlpha > targetEffectAlpha) effectAlpha = Math.Max(targetEffectAlpha, effectAlpha-fragment);
                if(alphaMod < targetAlphaMod) alphaMod = Math.Min(targetAlphaMod, alphaMod+fragment);
                if(alphaMod > targetAlphaMod) alphaMod = Math.Max(targetAlphaMod, alphaMod-fragment);
            } else {
                alpha = TargetAlpha;
                effectAlpha = targetEffectAlpha;
                alphaMod = targetAlphaMod;
            }

            // gradualy move the scale value towards target scale
            if(MillisPerScale > 0) {
                fragment = (float)time.ElapsedGameTime.Milliseconds/MillisPerScale;
                if(scale < targetScale) scale = Math.Min(targetScale, scale+fragment);
                if(scale > targetScale) scale = Math.Max(targetScale, scale-fragment);
                if(effectScale < targetEffectScale) effectScale = Math.Min(targetEffectScale, effectScale+fragment);
                if(effectScale > targetEffectScale) effectScale = Math.Max(targetEffectScale, effectScale-fragment);
            } else {
                scale = targetScale;
                effectScale = targetEffectScale;
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
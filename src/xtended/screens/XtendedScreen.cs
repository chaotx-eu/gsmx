namespace GSMXtended {
    using GameStateManagement;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// The base class of all screens in the
    /// xtended library
    public class XtendedScreen : GameScreen {
        /// The main container of this screen
        public Container MainContainer{get;}

        /// A blank texture which may be used for darkening
        /// background screens or fading in/out content
        public Texture2D BlankTexture {get; set;}

        /// Content manager of this screen
        public ContentManager Content {get; set;}

        /// The alpha value of this screen and all its content
        private float alpha, targetAlpha = 1f;
        public float Alpha {
            get {return alpha;}
            set {
                alpha = value;
                applyAlpha(MainContainer, value);
            }
        }

        /// Width of the screen
        public int Width { get {
            return ScreenManager
                .GraphicsDevice
                .Viewport.Width;
        }}

        /// Height of the screen
        public int Height { get {
            return ScreenManager
                .GraphicsDevice
                .Viewport.Height;
        }}

        /// Shared sprite batch meant to be used for images
        public SpriteBatch ImageBatch {get; set;}

        /// Shared sprite batch meant to be used fo text
        public SpriteBatch FontBatch {get; set;}

        /// Creates a new XtendedScreen object
        public XtendedScreen(Container mainContainer) {
            MainContainer = mainContainer;
            MainContainer.ParentScreen = this;
        }

        /// Loads required contend for this screen and its components
        public override void LoadContent() {
            if(Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            ImageBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            FontBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            BlankTexture = Content.Load<Texture2D>("images/blank");
            MainContainer.load();
        }

        /// Updates the main container and its components
        public override void Update(GameTime gameTime,
        bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
            if(!otherScreenHasFocus)
                MainContainer.update(gameTime);
        }

        /// Draws the main container and its components
        public override void Draw(GameTime gameTime) {
            ImageBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            FontBatch.Begin();
            MainContainer.draw();
            ImageBatch.End();
            FontBatch.End();
        }

        /// Handles user input for this screen and its components
        /// but only if it is active and focused
        public override void HandleInput(InputState inputState) {
            MainContainer.handleInput(inputState);
        }

        /// Helper to apply same alpha value to a screen component
        /// and all its children in case it is a container (recursive)
        protected void applyAlpha(ScreenComponent component, float alpha) {
            component.Alpha = alpha;

            if(component is Container)
                foreach(ScreenComponent child in ((Container)component).Children)
                    applyAlpha(child, alpha);
        }
    }
}
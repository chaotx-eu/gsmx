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

        /// The alpha value of this screen which will be
        /// applied to its MainContainer and all its children
        public float Alpha {get; set;} = 1;

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

            // Since the alignment property has no effect within
            // a screen this should always be the prefered setting
            MainContainer.PercentWidth = MainContainer.PercentHeight = 100;
        }

        /// Loads required contend for this screen and its components
        public override void LoadContent() {
            if(Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");
                
            ImageBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            FontBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            BlankTexture = Content.Load<Texture2D>("images/blank");
            MainContainer.load();
            MainContainer.init();
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
    }
}
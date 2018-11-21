namespace GameStateManagement {
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public class BackgroundScreen : GameScreen {
        private string imagePath;
        private Texture2D image;
        private ContentManager content;

        public BackgroundScreen(string imagePath) {
            this.imagePath = imagePath;
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void LoadContent() {
            if(content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            image = content.Load<Texture2D>(imagePath);
        }

        public override void UnloadContent() {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherFocused, bool covered) {
            base.Update(gameTime, otherFocused, false);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch batch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle rectangle = new Rectangle(0, 0, viewport.Width, viewport.Height);

            batch.Begin();
            batch.Draw(image, rectangle, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            batch.End();
        }
    }
}
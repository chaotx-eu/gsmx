namespace GSMXtended {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// An item which is represented by an image
    public class ImageItem : MenuItem {
        /// Location of the image file
        public string ImageFile {get; set;}

        /// Source rectangle within the image, setting this
        /// will override the width and height attribute
        private Rectangle? sourceRectangle;
        public Rectangle? SourceRectangle {
            get {return sourceRectangle;}
            set {
                sourceRectangle = value;
                Width = value.HasValue ? value.Value.Width
                    : Image != null ? Image.Width : -1;
                Height = value.HasValue ? value.Value.Height
                    : Image != null ? Image.Height : -1;                    
            }
        }

        /// Reference to the loaded image object
        public Texture2D Image {get; set;}

        /// The total width in pixels when the image is drawn
        private int width;
        public override int Width {
            get {return (int)(width*Scale);}
            set {width = value;}
        }

        /// The total height in pixels when the image is drawn
        private int height;
        public override int Height {
            get {return (int)(height*Scale);}
            set {height = value;}
        }

        /// Creates a new image object
        public ImageItem(string imageFile)
        : this(imageFile, null) {}

        public ImageItem(string imageFile, Rectangle? sourceRectangle) {
            ImageFile = imageFile;
            SourceRectangle = sourceRectangle;
            PercentWidth = PercentHeight = -1; // no effect on ImageItem
        }

        /// Loads the image file
        public override void load() {
            base.load();
            ContentManager content =  ParentScreen.ScreenManager.Game.Content;
            Image = content.Load<Texture2D>(ImageFile);

            // checks if size attributes are valid and fixes them if not
            if(Width < 0) Width = SourceRectangle.HasValue
                ? SourceRectangle.Value.Width : Image.Width;

            if(Height < 0) Height = SourceRectangle.HasValue
                ? SourceRectangle.Value.Height : Image.Height;
        }

        /// Draws the image to the screen
        public override void draw() {
            base.draw();
            Vector2 origin = Vector2.Zero; // TODO implement rotation
            SpriteBatch batch = ParentScreen.ImageBatch;
            batch.Draw(Image, scaledRectangle((int)X, (int)Y, Width, Height, EffectScale),
                SourceRectangle, Color, Rotation, origin, SpriteEffects.None, 1f);
        }

        /// Creates a scaled rectangle and returns it, taking
        /// the current alignment of this item into accont
        //  => TODO move to MapUtils?
        private Rectangle scaledRectangle(int x, int y, int width, int height, float scale) {
            return new Rectangle(
                x - (HAlignment == HAlignment.Left ? 0
                    : HAlignment == HAlignment.Right
                    ? (int)(width*scale - width)
                    : (int)(width*scale - width)/2),
                y - (VAlignment == VAlignment.Top ? 0
                    : VAlignment == VAlignment.Bottom
                    ? (int)(height*scale - height)
                    : (int)(height*scale - height)/2),
                (int)(width*scale), (int)(height*scale)
            );
        }
    }
}
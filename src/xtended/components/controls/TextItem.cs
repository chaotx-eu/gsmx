namespace GSMXtended {
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// An item which is represented by text
    public class TextItem : MenuItem {
        /// The text to be displayed
        public string Text {get; set;}

        /// Location of the font file
        public string FontFile {get; set;}

        /// The font used for the text
        public SpriteFont Font {get; set;}

        /// Item width with in pixels with scaling applied
        public override int Width {
            get {return (int)(Font.MeasureString(Text).X*Scale);}
        }

        /// Item height in pixels with scaling applied
        public override int Height {
            get {return (int)(Font.MeasureString(Text).Y*Scale);}
        }

        /// Width in pixels of this text without any scaling applied
        public override int BaseWidth {
            get {return (int)(Font.MeasureString(Text).X);}
        }

        /// Height in pixels of this text without any scaling applied
        public override int BaseHeight {
            get {return (int)(Font.MeasureString(Text).Y);}
        }

        public TextItem() : this("") {}
        public TextItem(string text) {
            Text = text;
            PercentWidth = PercentHeight = -1;
        }

        public TextItem(string text, string fontFile) {
            Text = text;
            FontFile = fontFile;
            PercentWidth = PercentHeight = -1;
        }

        public TextItem(string text, SpriteFont font) : this(text, "") {
            Font = font;
        }

        /// Loads the font file
        public override void load() {
            base.load();
            if(Font == null) Font =
                ParentScreen.ScreenManager.Game
                .Content.Load<SpriteFont>(FontFile);
        }

        /// Draws the text to the screen
        public override void draw() {
            base.draw();
            Vector2 origin = Vector2.Zero; // TODO rotation
            Vector2 position = new Vector2(
                X - (HAlignment == HAlignment.Left ? 0
                    : HAlignment == HAlignment.Right
                    ? (Width*EffectScale - Width)
                    : (Width*EffectScale - Width)/2),
                Y - (VAlignment == VAlignment.Top ? 0
                    : VAlignment == VAlignment.Bottom
                    ? (Height*EffectScale - Height)
                    : (Height*EffectScale - Height)/2));

            SpriteBatch batch = ParentScreen.FontBatch;
            batch.DrawString(Font, Text, position, Color,
                Rotation, origin, Scale*EffectScale, SpriteEffects.None, 1f);
        }
    }
}
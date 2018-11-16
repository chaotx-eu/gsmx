namespace GSMXtended {
    using System;
    using System.Diagnostics;
    using Microsoft.Xna.Framework;

    /// An item which may be an selectable object
    /// within a container with a menu structure
    public class MenuItem : Control {
        /// Primary color of this item. If the item is selected
        /// the secondary color will be returned
        public override Color Color {
            get {return IsSelected ?
                SecondaryColor*Alpha : base.Color;
            }
            set {base.Color = value;}
        }

        /// Secondary color of this item
        public Color SecondaryColor {get; set;}

        // TODO
        /// The padding of this item. Will not be affected
        /// by scaling e.g. if width*scale > basewidth + padding
        /// no padding will be applied
        // public Vector4 Padding {get; set;}

        /// The scaling of this item
        private float scale, targetScale = 1f;
        public virtual float Scale {
            get {return scale;}
            set {targetScale = value;}
        }

        /// Scaling used for effects and similiar, size
        /// attributes must be mutliplied with this on demand
        private float effectScale, targetEffectScale = 1f;
        public float EffectScale {
            get {return effectScale + ScaleMod;}
            set {targetEffectScale = value;}
        }

        /// Width of the item in pixels without any scaling applied
        public virtual int BaseWidth {get {return Width;}}

        /// Height of the item in pixels without any scaling applied
        public virtual int BaseHeight {get {return Height;}}

        /// The rotation of this item
        /// TODO -> currently not implemented (too lazy atm)
        ///      -> see BesmashContent.MapObject for how its done
        public virtual float Rotation {get;} = 0f;

        // Wether this Item is currently selected
        public bool IsSelected {
            get {return isSelected && ((MenuList)ParentContainer).IsFocused;}
            set {isSelected = value;}
        }

        /// How many milliseconds it would take to scale
        /// this item from 0 to 1, values below 0 are
        /// equivalent to 0 (for now this will also
        /// affect the alpha value)
        public int MillisPerScale {get; set;} = 456;

        /// Scaling modificator. Will be added to scale
        protected virtual float ScaleMod {get; set;} = 0f;

        private bool isSelected;
        private float selectionFade;

        public MenuItem() {
            // default primary color
            Color = Color.White;
        }

        public override void update(GameTime gameTime) {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state. (Copied from GameStateManagement.MenuEntry)
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds*4;
            selectionFade = IsSelected
                ? Math.Min(selectionFade + fadeSpeed, 1)
                : Math.Max(selectionFade - fadeSpeed, 0);

            // This should probably be done in the draw method (TODO)
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            ScaleMod = pulsate * 0.05f * selectionFade;
            ////////////////////////////////////////////////////////////////////////

            // gradualy move the scale value towards target scale
            if(MillisPerScale > 0) {
                float fragment = (float)gameTime.ElapsedGameTime.Milliseconds/MillisPerScale;
                if(scale < targetScale) scale = Math.Min(targetScale, scale+fragment);
                if(scale > targetScale) scale = Math.Max(targetScale, scale-fragment);
                if(effectScale < targetEffectScale) effectScale = Math.Min(targetEffectScale, effectScale+fragment);
                if(effectScale > targetEffectScale) effectScale = Math.Max(targetEffectScale, effectScale-fragment);
            } else {
                scale = targetScale;
                effectScale = targetEffectScale;
            }

            SecondaryColor = ((MenuList)ParentContainer).SelectedColor;
            base.update(gameTime);
        }
    }
}
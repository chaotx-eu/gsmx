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

        /// Scaling used for effects and similiar, size
        /// attributes may be mutliplied with this on demand
        public override float EffectScale {
            get {return base.EffectScale + ScaleMod;}
            set {base.EffectScale = value;}
        }

        /// Width of the item in pixels without any scaling applied
        public virtual int BaseWidth {get {return Width;}}

        /// Height of the item in pixels without any scaling applied
        public virtual int BaseHeight {get {return Height;}}

        /// The rotation of this item
        /// TODO -> currently not implemented (too lazy atm)
        ///      -> see BesmashContent.MapObject for how its done
        public virtual float Rotation {get;} = 0f;

        /// Wether this item is currently selected
        public bool IsSelected {
            get {return isSelected
                && (!(ParentContainer is MenuList)
                || ((MenuList)ParentContainer).IsFocused);}
            set {isSelected = value;}
        }

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

            if(ParentContainer is MenuList)
                SecondaryColor = ((MenuList)ParentContainer).SelectedColor;
                
            base.update(gameTime);
        }
    }
}
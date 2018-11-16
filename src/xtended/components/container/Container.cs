namespace GSMXtended {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// Defines the signature of AlignmentStrategies
    public delegate void AlignmentStrategy(Collection<ScreenComponent> components);

    /// Base class for all containers in this lib
    /// capable of holding and managing other components
    public class Container : ScreenComponent {
        private XtendedScreen parentScreen;
        public override XtendedScreen ParentScreen {
            get {return parentScreen;}
            set{
                parentScreen = value;
                Children.ToList().ForEach(c => c.ParentScreen = value);
            }
        }

        /// The components within this container
        private List<ScreenComponent> children;
        public ReadOnlyCollection<ScreenComponent> Children {
            get { return children == null
                    ? (children = new List<ScreenComponent>()).AsReadOnly()
                    : children.AsReadOnly();
            }
        }

        /// Bakground texture of this container
        public Texture2D Texture {get; set;}

        /// The path to the file with the background texture
        public string TextureFile {get; set;}

        /// Spacing in pixels between components in this container
        public virtual int Spacing {get; set;} // TODO

        public Container(params ScreenComponent[] items) {
            add(items);
        }

        /// Adds passed component(s) to this container
        public void add(params ScreenComponent[] items) {
            if(children == null) children = new List<ScreenComponent>();
            items.ToList().ForEach(i => {
                i.ParentContainer = this;
                i.ParentScreen = ParentScreen;
                children.Add(i);
            });
        }

        /// Removes passed component(s) from this container
        public void remove(params ScreenComponent[] items) {
            if(children == null) return;
            items.ToList().ForEach(i => children.Remove(i));
        }

        /// Loads required ressources for components in
        /// this container
        public override void load() {
            base.load();

            Texture = TextureFile == null ? ParentScreen.BlankTexture
                : ParentScreen.Content.Load<Texture2D>(TextureFile);

            Children.ToList().ForEach(c => c.load());
        }

        /// Draws all compononents within this container
        public override void draw() {
            base.draw();
            Rectangle destination = new Rectangle((int)X, (int)Y, Width, Height);
            SpriteBatch batch = ParentScreen.ImageBatch;
            batch.Draw(Texture, destination, null, Color);
            Children.ToList().ForEach(c => c.draw());
        }

        /// Updates all managed components within this container
        /// and its own size aswell if Managed is true and the
        /// ParentContainer is null
        public override void update(GameTime time) {
            if(Managed && ParentContainer == null) {
                if(PercentWidth >= 0) Width =
                    (int)((PercentWidth/100f)*ParentScreen.Width);

                if(PercentHeight >= 0) Height =
                    (int)((PercentHeight/100f)*ParentScreen.Height);
            }

            Children.Where(c => c.Managed).ToList().ForEach(child => {
                if(child.PercentWidth >= 0)
                    child.Width = (int)((child.PercentWidth/100f)*Width);

                if(child.PercentHeight >= 0)
                    child.Height = (int)((child.PercentHeight/100f)*Height);
                    
                onUpdate(child);
                child.update(time);
            });

            base.update(time);
        }

        /// Gets called on each child within the update method
        protected virtual void onUpdate(ScreenComponent child) {}
    }
}
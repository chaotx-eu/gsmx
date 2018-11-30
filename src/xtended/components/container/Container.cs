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
        // TODO test
        public override float Scale {
            get {return base.Scale;}
            set {base.Scale = value;}
        }

        public override int Width {
            get {return (int)(base.Width*
                (PercentWidth > 0 ? Scale : 1));}
        }

        public override int Height {
            get {return (int)(base.Height*
                (PercentHeight > 0 ? Scale : 1));}
        }
        ////////////

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

        /// Background texture of this container
        public Texture2D Texture {get; set;}

        /// The path to the file with the background texture
        public string TextureFile {get; set;}

        /// Spacing in pixels between components in this container
        public virtual int Spacing {get; set;} // TODO

        public Container(params ScreenComponent[] items) {
            add(items);
        }

        /// Loads required ressources for components in
        /// this container
        public override void load() {
            base.load();
            Texture = TextureFile == null ? ParentScreen.BlankTexture
                : ParentScreen.Content.Load<Texture2D>(TextureFile);

            Children.ToList().ForEach(c => c.load());
        }

        /// Initializes this container and its components
        public override void init() {
            base.init();
            Children.ToList().ForEach(c => c.init());
        }

        /// Draws all compononents within this container
        public override void draw() {
            base.draw();
            Rectangle destination = new Rectangle((int)X, (int)Y, Width, Height);
            SpriteBatch batch = ParentScreen.ImageBatch;
            batch.Draw(Texture, destination, null, Color);
            Children.ToList().ForEach(c => c.draw());
        }

        /// Updates itself and all components within this container
        public override void update(GameTime time) {
            align();
            Children.ToList().ForEach(child => {
                alignChild(child);
                onUpdate(child);
                child.update(time);
            });
            base.update(time);
        }

        /// Adds passed component(s) to this container
        public virtual void add(params ScreenComponent[] items) {
            if(children == null) children = new List<ScreenComponent>();
            items.ToList().ForEach(i => {
                i.ParentContainer = this;
                i.ParentScreen = ParentScreen;
                children.Add(i);
            });
        }

        /// Removes passed component(s) from this container
        public virtual void remove(params ScreenComponent[] items) {
            if(children == null) return;
            items.ToList().ForEach(i => children.Remove(i));
        }

        /// Sets the size of this container and aligns
        /// it relative to its parent
        public virtual void align() {
            if(ParentContainer == null) {
                if(PercentWidth >= 0) Width =
                    ((int)((PercentWidth/100f)*ParentScreen.Width));

                if(PercentHeight >= 0) Height =
                    ((int)((PercentHeight/100f)*ParentScreen.Height));
            }
        }

        /// Helper to align all children and set their size
        public virtual void alignChildren() {
            Children.ToList().ForEach(child => {
                alignChild(child);
                if(child is Container) {
                    ((Container)child).align();
                    ((Container)child).alignChildren();
                }
            });
        }

        /// Sets the size and aligns the children
        /// of this container during the update method
        protected virtual void alignChild(ScreenComponent child) {
            if(child.PercentWidth >= 0)
                child.Width = (int)((child.PercentWidth/100f)*Width);

            if(child.PercentHeight >= 0)
                child.Height = (int)((child.PercentHeight/100f)*Height);
        }

        /// Gets called on each child within the update method
        protected virtual void onUpdate(ScreenComponent child) {}

        /// Helper to apply alpha value to a screen component and
        /// in case it is a container to all its children (recursive)
        public static void applyAlpha(ScreenComponent child, float alpha) {
            child.Alpha = alpha;

            if(child is Container) ((Container)child)
                .Children.ToList()
                .ForEach(c => applyAlpha(c, alpha));
        }

        /// Helper to apply scale value to a screen component and
        /// in case it is a container to all its children (recursive)
        public static void applyScale(ScreenComponent child, float scale) {
            child.Scale = scale*child.DefaultScale;
            if(child is Container) ((Container)child)
                .Children.ToList()
                .ForEach(c => applyScale(c, scale));
        }

        /// Helper to apply effect alpha to a screen component and
        /// in case it is a container to all its children (recursive)
        public static void applyEffectAlpha(ScreenComponent child, float alpha) {
            child.EffectAlpha = alpha;
            if(child is Container) ((Container)child)
                .Children.ToList()
                .ForEach(c => applyEffectAlpha(c, alpha));
        }

        /// Helper to apply effect scale to a screen component and
        /// in case it is a container to all its children (recursive)
        public static void applyEffectScale(ScreenComponent child, float scale) {
            child.EffectScale = scale;
            if(child is Container) ((Container)child)
                .Children.ToList()
                .ForEach(c => applyEffectScale(c, scale));
        }

        /// Helper to align child within a parent container horizontally
        public static void hAlign(Container parent, ScreenComponent child) {
            float x;
            if(child.HAlignment == HAlignment.Left) {
                x = parent.X;

                parent.Children.Where(
                    c => parent.Children.IndexOf(c) < parent.Children.IndexOf(child)
                    && c.HAlignment == HAlignment.Left).ToList()
                        .ForEach(c => x += c.Width);
            } else if(child.HAlignment == HAlignment.Right) {
                x = parent.X + parent.Width - child.Width;

                parent.Children.Where(
                    c => parent.Children.IndexOf(c) > parent.Children.IndexOf(child)
                    && c.HAlignment == HAlignment.Right).ToList()
                        .ForEach(c => x -= c.Width);
            } else { // default: centered
                int lw = 0, rw = 0, cw = child.Width;

                parent.Children.Where(c => c.HAlignment != HAlignment.Left
                && c.HAlignment != HAlignment.Right).ToList().ForEach(c => {
                    if(parent.Children.IndexOf(c) < parent.Children.IndexOf(child))
                        lw += c.Width;

                    if(parent.Children.IndexOf(c) > parent.Children.IndexOf(child))
                        rw += c.Width;
                });

                x = (parent.X + parent.Width/2f) - (lw + rw + cw)/2f + lw;
                // Example:
                //        V (target x = 5), (Width := 17)
                // [  ||| ||| || ||||| (13 -> lw = 3, rw = 7)  ]
                //         ^ target child (cw = 3)
                //
                // x = Width/2 - (lw + rw + cw)/2 + lw
                //   = (17/2) - (13/2) + 3 = 8 - 6 + 3 = 5
            }

            child.X = x;
            child.Y = child.VAlignment == VAlignment.Bottom
                ? parent.Y + parent.Height - child.Height
                : child.VAlignment == VAlignment.Top ? parent.Y
                : parent.Y + parent.Height/2f - child.Height/2f;
        }

        /// Helper to align a child within a parent container verticallly
        public static void vAlign(Container parent, ScreenComponent child) {
            float y;

            if(child.VAlignment == VAlignment.Top) {
                y = parent.Y;

                parent.Children.Where(
                    c => parent.Children.IndexOf(c) < parent.Children.IndexOf(child)
                    && c.VAlignment == VAlignment.Top).ToList()
                        .ForEach(c => y += c.Height);
            } else if(child.VAlignment == VAlignment.Bottom) {
                y = parent.Y + parent.Height - child.Height;

                parent.Children.Where(
                    c => parent.Children.IndexOf(c) > parent.Children.IndexOf(child)
                    && c.VAlignment == VAlignment.Bottom).ToList()
                        .ForEach(c => y -= c.Height);
            } else { // default: centered
                int th = 0, bh = 0, ch = child.Height;

                parent.Children.Where(c => c.VAlignment != VAlignment.Top
                && c.VAlignment != VAlignment.Bottom).ToList().ForEach(c => {
                    if(parent.Children.IndexOf(c) < parent.Children.IndexOf(child))
                        th += c.Height;

                    if(parent.Children.IndexOf(c) > parent.Children.IndexOf(child))
                        bh += c.Height;
                });

                y = (parent.Y + parent.Height/2f) - (th + bh + ch)/2f + th;
            }

            child.Y = y;
            child.X = child.HAlignment == HAlignment.Right
                ? parent.X + parent.Width - child.Width
                : child.HAlignment == HAlignment.Left ? parent.X
                : parent.X + parent.Width/2f - child.Width/2f;
        }
    }
}
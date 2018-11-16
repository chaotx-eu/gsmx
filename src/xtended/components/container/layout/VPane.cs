namespace GSMXtended {
    using System.Linq;
    using System;
    using Microsoft.Xna.Framework;

    /// Aligns its children vertically according to
    /// their set H- and VAlignment-Property where
    /// VAlignment may have an effect on the order
    /// of the items HAlignment will only align the
    /// item within its own row.
    public class VPane : Container {
        public VPane(params ScreenComponent[] items) {
            PercentWidth = PercentHeight = -1;
            add(items);
        }

        /// Updates height of this VPane
        public override void update(GameTime time) {
            if(Managed && (PercentHeight < 0 || PercentWidth < 0)) {
                int w = 0, h = 0;
                Children.ToList().ForEach(c => {
                    h += c.Height;
                    w = w < c.Width ? c.Width : w;
                });

                if(PercentHeight < 0) Height = h;
                if(PercentWidth < 0) Width = w;
            }

            base.update(time);
        }

        /// Aligns children vertically (similar to VBox in JavaFX)
        protected override void onUpdate(ScreenComponent child) {
            align(this, child);
        }

        /// Aligns the child within the parent with the policy of a Vpane
        public static void align(Container parent, ScreenComponent child) {
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
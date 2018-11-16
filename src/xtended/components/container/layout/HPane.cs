namespace GSMXtended {
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// Aligns its children horizontally according to
    /// their set H- and VAlignment-Property where
    /// HAlignment may have an effect on the order
    /// of the items VAlignment will only align the
    /// item within its own column.
    public class HPane : Container {
        public HPane(params ScreenComponent[] items) {
            PercentWidth = PercentHeight = -1;
            add(items);
        }

        /// Updates width of this HPane
        public override void update(GameTime time) {
            if(Managed && (PercentWidth < 0 || PercentHeight < 0)) {
                int w = 0, h = 0;
                Children.ToList().ForEach(c => {
                    w += c.Width;
                    h = h < c.Height ? c.Height : h;
                });

                if(PercentWidth < 0) Width = w;
                if(PercentHeight < 0) Height = h;
            }

            base.update(time);
        }

        /// Aligns children horizontally (similar to HBox in JavaFX)
        protected override void onUpdate(ScreenComponent child) {
            align(this, child);
        }

        /// Aligns the child within the parent with the policy of a Vpane
        public static void align(Container parent, ScreenComponent child) {
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
    }
}
namespace GSMXtended {
    using System.Linq;
    using System;
    using Microsoft.Xna.Framework;

    /// Aligns its children vertically according to
    /// their set H- and VAlignment-Property where
    /// VAlignment may have an effect on the order
    /// of the items and HAlignment will align the
    /// item only within its own row.
    public class VPane : Container {
        public VPane(params ScreenComponent[] items) {
            PercentWidth = PercentHeight = -1;
            add(items);
        }

        /// Updates height of this VPane
        public override void align() {
            base.align();
            if(PercentHeight < 0 || PercentWidth < 0) {
                int w = 0, h = 0;
                Children.ToList().ForEach(c => {
                    h += c.Height;
                    w = w < c.Width ? c.Width : w;
                });

                if(PercentHeight < 0) Height = h;
                if(PercentWidth < 0) Width = w;
            }
        }

        /// Aligns children vertically (similar to VBox in JavaFX)
        protected override void alignChild(ScreenComponent child) {
            base.alignChild(child);
            Container.vAlign(this, child);
        }
    }
}
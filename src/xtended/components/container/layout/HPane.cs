namespace GSMXtended {
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// Aligns its children horizontally according to
    /// their set H- and VAlignment-Property where
    /// HAlignment may have an effect on the order
    /// of the items and VAlignment will align the
    /// item only within its own column.
    public class HPane : Container {
        public HPane(params ScreenComponent[] items) {
            PercentWidth = PercentHeight = -1;
            add(items);
        }

        /// Updates width of this HPane
        public override void align() {
            base.align();
            if(PercentWidth < 0 || PercentHeight < 0) {
                int w = 0, h = 0;
                Children.ToList().ForEach(c => {
                    w += c.Width;
                    h = h < c.Height ? c.Height : h;
                });

                if(PercentWidth < 0) Width = w;
                if(PercentHeight < 0) Height = h;
            }
        }

        /// Aligns children horizontally (similar to HBox in JavaFX)
        protected override void alignChild(ScreenComponent child) {
            base.alignChild(child);
            Container.hAlign(this, child);
        }
    }
}
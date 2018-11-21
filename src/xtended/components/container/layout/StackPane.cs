namespace GSMXtended {
    using System.Linq;
    
    /// Stacks its components on top of eachother
    /// in an area large so that the largest child fits
    public class StackPane : Container {
        public StackPane(params ScreenComponent[] items) {
            PercentWidth = PercentHeight = -1;
            add(items);
        }

        /// Updates width of this StackPane
        public override void align() {
            base.align();
            if(Managed && (PercentWidth < 0 || PercentHeight < 0)) {
                int w = 0, h = 0;
                Children.ToList().ForEach(c => {
                    w = w < c.Width ? c.Width : w;
                    h = h < c.Height ? c.Height : h;
                });

                if(PercentWidth < 0) Width = w;
                if(PercentHeight < 0) Height = h;
            }
        }

        /// Aligns children according to ther H- and VAlignment
        protected override void alignChild(ScreenComponent child) {
            base.alignChild(child);
            child.X = child.HAlignment == HAlignment.Right
                ? X + Width - child.Width
                : child.HAlignment == HAlignment.Left ? X
                : X + Width/2f - child.Width/2f;

            child.Y = child.VAlignment == VAlignment.Bottom
                ? Y + Height - child.Height
                : child.VAlignment == VAlignment.Top ? Y
                : Y + Height/2f - child.Height/2f;
        }
    }
}
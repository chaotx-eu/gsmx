namespace GSMXtended {
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    
    /// Contains a horizontal list of components and
    /// offers the ability to select and activate them
    public class HList : MenuList {
        public HList(params ScreenComponent[] items) : base(items) {
            // default control keys/buttons
            ControlNextKeys = new List<Keys>(new Keys[]{Keys.Right});
            ControlNextButtons = new List<Buttons>(new Buttons[]{Buttons.DPadRight, Buttons.LeftThumbstickRight});
            ControlPreviousKeys = new List<Keys>(new Keys[]{Keys.Left});
            ControlPreviousButtons = new List<Buttons>(new Buttons[]{Buttons.DPadLeft, Buttons.LeftThumbstickLeft});
        }

        /// Updates width of this HPane
        public override void align() {
            base.align();
            if(PercentWidth < 0 || PercentHeight < 0) {
                // width always as small as possible
                int w = 0, wMax = 0, hMax = 0;
                foreach(ScreenComponent c in Children) {
                    int index_child = Children.IndexOf(c);
                    int index_selected = SelectedItem != null ? Children.IndexOf(SelectedItem) : -1;
                    int diff = index_selected < 0 ? 0 : Math.Abs(index_selected - index_child);
                    if(IsStatic || c == SelectedItem || diff <= VisibleRange)
                        w += c.Width;

                    wMax = wMax < c.Width ? c.Width : wMax;
                    hMax = hMax < c.Height ? c.Height : hMax;
                }

                if(PercentWidth < 0) Width = IsStatic ? w : wMax;
                if(PercentHeight < 0) Height = hMax;
            }
        }

        protected override void alignChild(ScreenComponent child) {
            base.alignChild(child);
            
            if(IsStatic) {
                Container.hAlign(this, child);
                return;
            }

            Container.hAlign(this, child);
            if(child == SelectedItem) {
                Container.applyScale(child, Scale);
                Container.applyEffectAlpha(child, EffectAlpha);
                child.HAlignment = HAlignment.Center;
            } else if(SelectedItem != null) {
                // realign item x-coordinate relative to selected item
                int item_index = Children.IndexOf(child);
                int selected_index = Children.IndexOf(SelectedItem);
                child.HAlignment = HAlignment.Left;

                ScreenComponent other = Children[item_index
                    + (item_index > selected_index ? -1 : 1)];

                child.X = other.X + (item_index > selected_index
                    ? other.Width : -child.Width);

                // fade and scale items out of range away
                int diff = Math.Abs(selected_index - item_index);
                float alpha = 1f - Math.Min(1f, (float)diff/(VisibleRange+1));
                Container.applyScale(child, alpha*Scale);
                Container.applyEffectAlpha(child, alpha*EffectAlpha);
            }
        }
    }
}
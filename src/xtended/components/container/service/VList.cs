namespace GSMXtended {
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// Contains a vertical list of components and
    /// offers the ability to select and activate them
    public class VList : MenuList {
        public VList(params ScreenComponent[] items) : base(items) {
            IsStatic = true;

            // default selection keys/buttons
            ControlNextKeys = new List<Keys>(new Keys[]{Keys.Down});
            ControlNextButtons = new List<Buttons>(new Buttons[]{Buttons.DPadDown, Buttons.LeftThumbstickDown});
            ControlPreviousKeys = new List<Keys>(new Keys[]{Keys.Up});
            ControlPreviousButtons = new List<Buttons>(new Buttons[]{Buttons.DPadUp, Buttons.LeftThumbstickUp});
        }
        
        public override void align() {
            base.align();
            if(PercentHeight < 0 || PercentWidth < 0) {
                // height always as small as possible
                int h = 0, wMax = 0, hMax = 0;
                foreach(ScreenComponent c in Children) {
                    int index_child = Children.IndexOf(c);
                    int index_selected = SelectedItem != null ? Children.IndexOf(SelectedItem) : -1;
                    int diff = index_selected < 0 ? 0 : Math.Abs(index_selected - index_child);
                    if(IsStatic || c == SelectedItem || diff <= VisibleRange)
                        h += c.Height;

                    wMax = wMax < c.Width ? c.Width : wMax;
                    hMax = hMax < c.Height ? c.Height : hMax;
                }

                if(PercentHeight < 0) Height = IsStatic ? h : hMax;
                if(PercentWidth < 0) Width = wMax;
            }
        }
        
        protected override void alignChild(ScreenComponent child) {
            base.alignChild(child);
            
            if(IsStatic) {
                Container.vAlign(this, child);
                return;
            }

            Container.vAlign(this, child);
            if(child == SelectedItem) {
                Container.applyScale(child, Scale);
                Container.applyEffectAlpha(child, EffectAlpha);
                child.VAlignment = VAlignment.Center;
            } else if(child != null) {
                // realign item y-coordinate relative to selected item
                int item_index = Children.IndexOf(child);
                int selected_index = Children.IndexOf(SelectedItem);
                child.VAlignment = VAlignment.Top;

                ScreenComponent other = Children[item_index
                    + (item_index > SelectedIndex ? -1 : 1)];

                child.Y = other.Y + (item_index > selected_index
                    ? other.Height : -child.Height);

                // fade and scale items out of range away
                int diff = Math.Abs(selected_index - item_index);
                float alpha = 1f - Math.Min(1f, (float)diff/(VisibleRange+1));
                Container.applyScale(child, alpha*Scale);
                Container.applyEffectAlpha(child, alpha*EffectAlpha);
            }
        }
    }
}
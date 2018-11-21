namespace GSMXtended {
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// Contains a vertical list of controls and
    /// offers the ability to select and activate them
    public class VList : MenuList {
        public VList(params Control[] items) : base(items) {
            IsStatic = true;

            // default selection keys/buttons
            ControlNextKeys = new List<Keys>(new Keys[]{Keys.Down});
            ControlNextButtons = new List<Buttons>(new Buttons[]{Buttons.DPadDown, Buttons.LeftThumbstickDown});
            ControlPreviousKeys = new List<Keys>(new Keys[]{Keys.Up});
            ControlPreviousButtons = new List<Buttons>(new Buttons[]{Buttons.DPadUp, Buttons.LeftThumbstickUp});
        }

        /// Public wrapper for the add method which only
        /// allows controls to be added others will be ignored
        public new void add(params ScreenComponent[] items) {
            foreach(ScreenComponent sc in items)
                if(sc is Control) base.add(sc);
        }
        
        public override void align() {
            base.align();
            if(Managed && (PercentHeight < 0 || PercentWidth < 0)) {
                // height always as small as possible
                int h = 0, wMax = 0, hMax = 0;
                foreach(Control c in Children) {
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
                VPane.align(this, child);
                return;
            }

            if(!(child is MenuItem)) return; // TODO
            MenuItem item = (MenuItem)child;
            VPane.align(this, item);

            if(item == SelectedItem) {
                item.EffectAlpha = 1f;
                item.EffectScale = 1f;
                item.VAlignment = VAlignment.Center;
            } else if(item != null) {
                // realign item y-coordinate relative to selected item
                int item_index = Children.IndexOf(child);
                int selected_index = Children.IndexOf(SelectedItem);
                item.VAlignment = VAlignment.Top;

                MenuItem other = (MenuItem)Children[item_index + (
                    item_index > SelectedIndex ? -1 : 1)];

                item.Y = other.Y + (item_index > selected_index
                    ? other.Height*(other == SelectedItem ? 1 : other.EffectScale)
                    : -item.Height*item.EffectScale);

                // fade and scale items out of range away
                int diff = Math.Abs(selected_index - item_index);
                float alpha = 1f - Math.Min(1f, (float)diff/(VisibleRange+1));
                item.EffectAlpha = alpha;
                item.EffectScale = alpha;
            }
        }
    }
}
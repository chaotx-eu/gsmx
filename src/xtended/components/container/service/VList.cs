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
            KeyPressedEvent += (sender, args) => {
                if(!IsFocused || InputLocked) return;
                if(args.KeyboardState != null && args.KeyboardState.Value.IsKeyDown(Keys.Up)
                || args.GamePadState != null && (args.GamePadState.Value.IsButtonDown(Buttons.DPadUp)
                || args.GamePadState.Value.IsButtonDown(Buttons.LeftThumbstickUp))) {
                    select(-1);
                    InputTimer = 0;
                }

                if(args.KeyboardState != null && args.KeyboardState.Value.IsKeyDown(Keys.Down)
                || args.GamePadState != null && (args.GamePadState.Value.IsButtonDown(Buttons.DPadDown)
                || args.GamePadState.Value.IsButtonDown(Buttons.LeftThumbstickDown))) {
                    select();
                    InputTimer = 0;
                }
            };
        }

        /// Public wrapper for the add method which only
        /// allows controls to be added others will be ignored
        public new void add(params ScreenComponent[] items) {
            foreach(ScreenComponent sc in items)
                if(sc is Control) base.add(sc);
        }

        private int timer;
        public override void update(GameTime time) {
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

            base.update(time);
        }

        protected override void onUpdate(ScreenComponent child) {
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
namespace GSMXtended {
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    
    /// Holds a list of items and draws them vertically
    /// aligned with the selected item beeing centered
    /// and the outer items fading out
    /// => TODO
    public class HList : MenuList {
        public HList(params Control[] items) : base(items) {
            KeyPressedEvent += (sender, args) => {
                if(!IsFocused || InputLocked) return;
                if(args.KeyboardState != null && args.KeyboardState.Value.IsKeyDown(Keys.Left)
                || args.GamePadState != null && (args.GamePadState.Value.IsButtonDown(Buttons.DPadLeft)
                || args.GamePadState.Value.IsButtonDown(Buttons.LeftThumbstickLeft))) {
                    select(-1);
                    InputTimer = 0;
                }

                if(args.KeyboardState != null && args.KeyboardState.Value.IsKeyDown(Keys.Right)
                || args.GamePadState != null && (args.GamePadState.Value.IsButtonDown(Buttons.DPadRight)
                || args.GamePadState.Value.IsButtonDown(Buttons.LeftThumbstickRight))) {
                    select();
                    InputTimer = 0;
                }
            };
        }

        /// Updates width of this HPane
        public override void update(GameTime time) {
            if(Managed && (PercentWidth < 0 || PercentHeight < 0)) {
                // width always as small as possible
                int w = 0, wMax = 0, hMax = 0;
                foreach(Control c in Children) {
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

            base.update(time);
        }

        protected override void onUpdate(ScreenComponent child) {
            if(IsStatic) {
                HPane.align(this, child);
                return;
            }

            if(!(child is MenuItem)) return; // for safety reasons (TODO)
            MenuItem item = (MenuItem)child;
            HPane.align(this, item);

            if(item == SelectedItem) {
                item.EffectAlpha = 1f;
                item.EffectScale = 1f;
                item.HAlignment = HAlignment.Center;
            } else if(SelectedItem != null) {
                // realign item x-coordinate relative to selected item
                int item_index = Children.IndexOf(item);
                int selected_index = Children.IndexOf(SelectedItem);
                item.HAlignment = HAlignment.Left;

                MenuItem other = (MenuItem)Children[item_index + (
                    item_index > selected_index ? -1 : 1)];

                item.X = other.X + (item_index > selected_index
                    ? other.Width*(other == SelectedItem ? 1 : other.EffectScale)
                    : -item.Width*item.EffectScale);

                // fade and scale items out of range away
                int diff = Math.Abs(selected_index - item_index);
                float alpha = 1f - Math.Min(1f, (float)diff/(VisibleRange+1));
                ((MenuItem)child).EffectAlpha = alpha;
                ((MenuItem)child).EffectScale = alpha;
            }
        }
    }
}
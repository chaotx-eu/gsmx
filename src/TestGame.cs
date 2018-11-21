namespace GSMXtended.Demo {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using GameStateManagement;
    using System;

    public class TestGame : Game {
        private ScreenManager screenManager;
        private GraphicsDeviceManager graphics;

        public TestGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // selection dialog
            TextItem dItem1 = new TextItem("FEEdffdwfw", "fonts/menufont");
            TextItem dItem2 = new TextItem("rfeefw3", "fonts/menufont");
            TextItem dItem3 = new TextItem("Yf4fef", "fonts/menufont");
            TextItem dItem4 = new TextItem("DVWEefw", "fonts/menufont");

            ImageItem dImage1 = new ImageItem("images/kevin_sheet", new Rectangle(0, 32, 16, 16));
            ImageItem dImage2 = new ImageItem("images/kevin_sheet", new Rectangle(0, 32, 16, 16));
            ImageItem dImage3 = new ImageItem("images/kevin_sheet", new Rectangle(0, 32, 16, 16));
            ImageItem dImage4 = new ImageItem("images/kevin_sheet", new Rectangle(0, 32, 16, 16));

            dImage1.Scale = 8;
            dImage2.Scale = 8;
            dImage3.Scale = 8;
            dImage4.Scale = 8;

            // HList hListD = new HList(dItem1, dItem2, dItem3, dItem4);
            HList hListD = new HList(dImage1, dImage2, dImage3, dImage4);
            HPane hDialog = new HPane(hListD);
            hDialog.Color = Color.Black;
            hDialog.Alpha = 0.5f;
            hDialog.PercentWidth = 100;
            hDialog.PercentHeight = 100;
            hListD.SelectedIndex = 0;
            hListD.IsFocused = true;
            // hListD.IsStatic = true;

            // layout
            TextItem item1 = new TextItem("ItemOne", "fonts/menufont");
            TextItem item2 = new TextItem("ItemTwo", "fonts/menufont");
            TextItem item3 = new TextItem("ItemThree", "fonts/menufont");
            TextItem item4 = new TextItem("ItemFour", "fonts/menufont");
            TextItem option4 = new TextItem("OptionFour", "fonts/menufont");
            TextItem option5 = new TextItem("OptionFive", "fonts/menufont");
            TextItem option6 = new TextItem("OptionSix", "fonts/menufont");
            TextItem option7 = new TextItem("OptionSeven", "fonts/menufont");
            TextItem option8 = new TextItem("OptionEight", "fonts/menufont");
            TextItem option9 = new TextItem("OptionNine", "fonts/menufont");
            TextItem optionFoo = new TextItem("Foo", "fonts/menufont");
            TextItem optionBar = new TextItem("Bar", "fonts/menufont");
            TextItem optionBaz = new TextItem("Baz", "fonts/menufont");
            // item1.HAlignment = HAlignment.Left;
            // item2.HAlignment = HAlignment.Left;
            // item3.HAlignment = HAlignment.Left;

            HList hList1 = new HList(option4);
            HList hList2 = new HList(option5, option6);
            HList hList3 = new HList(option7, option8, option9, optionFoo, optionBar, optionBaz);
            HList hList4 = new HList();
            hList3.VisibleRange = 2;
            hList4.VisibleRange = 3;
            hList1.SelectedIndex = 0;
            hList2.SelectedIndex = 0;
            hList3.SelectedIndex = 0;

            for(int i = 1; i < 11; ++i)
                hList4.add(new TextItem((i*10).ToString(), "fonts/menufont"));
            hList4.SelectedIndex = 0;

            VList vListItems = new VList(item1, item2, item3, item4);
            VPane vPaneOptions = new VPane(hList1, hList2, hList3, hList4);
            vListItems.VisibleRange = 3;
            vListItems.SelectedIndex = 0;
            vListItems.IsFocused = true;
            vListItems.IsStatic = false;
            vListItems.HAlignment = HAlignment.Left;
            vPaneOptions.HAlignment = HAlignment.Right;

            HPane hMain = new HPane(vListItems, vPaneOptions);
            hMain.PercentWidth = 100;
            hMain.PercentHeight = 100;

            XtendedDialog dialog = new XtendedDialog(hDialog);
            XtendedScreen mainScreen = new XtendedScreen(hMain);

            // event handler (synced focus)
            vListItems.SelectedEvent += (sender, args) => {
                if(args.SelectedItem == item1) hList1.IsFocused = true;
                if(args.SelectedItem == item2) hList2.IsFocused = true;
                if(args.SelectedItem == item3) hList3.IsFocused = true;
                if(args.SelectedItem == item4) hList4.IsFocused = true;
            };

            vListItems.DeselectedEvent += (sender, args) => {
                if(args.SelectedItem == item1) hList1.IsFocused = false;
                if(args.SelectedItem == item2) hList2.IsFocused = false;
                if(args.SelectedItem == item3) hList3.IsFocused = false;
                if(args.SelectedItem == item4) hList4.IsFocused = false;
            };
            
            vListItems.ActionEvent += (sender, args) => {
                Control item = args.SelectedItem;
                if(item == item1) hList1.IsFocused = true;
                if(item == item2) hList2.IsFocused = true;
                if(item == item3) hList3.IsFocused = true;
                if(item == item4) hList4.IsFocused = true;
                sender.IsFocused = false;
            };

            hList1.ActionEvent += (sender, args) => {
                Random rng = new Random();
                if(args.SelectedItem == option4) {
                    vListItems.Color = new Color(
                        rng.Next(255),
                        rng.Next(255),
                        rng.Next(255));
                }
            };

            hList2.ActionEvent += (sender, args) => {
                Control item = args.SelectedItem;
                if(item == option5) vListItems.Alpha = 1;
                if(item == option6) vListItems.Alpha = 0;
            };

            hList3.ActionEvent += (sender, args) => {
                Random rng = new Random();
                if(args.SelectedItem == option7) {
                    vPaneOptions.Color = new Color(
                        rng.Next(255),
                        rng.Next(255),
                        rng.Next(255));
                }

                if(args.SelectedItem == option8)
                    vPaneOptions.Alpha = Math.Abs(vPaneOptions.TargetAlpha-1);

                if(args.SelectedItem == option9) {
                    dialog.Alpha = 1;
                    hDialog.Alpha = 0.5f; // TODO => ...
                    hListD.IsFocused = true; // TODO => should gain focus on screen add
                    screenManager.AddScreen(dialog, null);
                }
            };

            hList1.CancelEvent += (sender, args) => {
                hList1.IsFocused = false;
                vListItems.IsFocused = true;
            };

            hList2.CancelEvent += (sender, args) => {
                hList2.IsFocused = false;
                vListItems.IsFocused = true;
            };

            hList3.CancelEvent += (sender, args) => {
                hList3.IsFocused = false;
                vListItems.IsFocused = true;
                // vListItems.select(2);
            };

            hListD.ActionEvent += (sender, args) => {
                if(args.SelectedItem == dItem3) {
                    dialog.Alpha = 0;
                    hListD.IsFocused = false; // TODO => should lose focus on screen exit
                    dialog.ExitScreen();      //         so InputTimer will reset
                }
            };

            hListD.CancelEvent += (sender, args) => {
                dialog.Alpha = 0;
                hListD.IsFocused = false;
                dialog.ExitScreen();
            };

            // init screen manager
            screenManager = new ScreenManager(this);
            screenManager.AddScreen(new BackgroundScreen("images/besmash_background"), null);
            screenManager.AddScreen(mainScreen, null);
            Components.Add(screenManager);
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            // nothing here
        }

        protected override void Update(GameTime gameTime) {
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}

namespace GSMXtended.Demo {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using GameStateManagement;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class ExampleGame : Game {
        private ScreenManager screenManager;
        private GraphicsDeviceManager graphics;

        public ExampleGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // A vertical list, which can contain screen components,
            // covering the half of the screen with its items centered
            VList vList = new VList();
            vList.IsFocused = true;
            vList.PercentWidth = 50;
            vList.PercentHeight = 50;
            vList.Color = Color.Green;
            vList.Alpha = 0.3f;
            vList.IsStatic = true;

            // A list may contain other lists. When another list
            // becomes selected/deselected it will gain/lose focus
            Random rng = new Random();
            for(int i = 0; i < 3; ++i) {
                // Components derived from MenuItem offer dedicated
                // functionality when added to MenuLists (e.g. effect
                // when selected while the MenuList is focused)
                HList hList = new HList(
                    new TextItem("Foo", "fonts/menu_font1"),
                    new TextItem("Bar", "fonts/menu_font1"),
                    new TextItem("Baz", "fonts/menu_font1")
                );

                hList.IsStatic = i%2 != 0;
                hList.PercentWidth = 50;
                hList.Color = new Color(rng.Next(256), rng.Next(256), rng.Next(256));

                // An event handler which changes the color or alpha of the horizontal
                // list when an ActionEvent is triggered (i.e. an item was clicked)
                hList.ActionEvent += (sender, args) => {
                    if(args.SelectedIndex == 0) hList.Color = Color.Orange;
                    if(args.SelectedIndex == 1) hList.Color = new Color(rng.Next(256), rng.Next(256), rng.Next(256));
                    if(args.SelectedIndex == 2) hList.Alpha = Math.Abs(hList.TargetAlpha-1);
                };

                vList.add(hList); // here we add the horizontal lists to the vertical list
            }

            // Antoher vertical list which will be put below the first one,
            // notice that this list has no focus (which is by default) (TODO focus bug)
            VList vList2 = new VList();
            vList2.PercentWidth = 50;
            vList2.PercentHeight = 50;
            vList2.Color = Color.Red;
            vList2.Alpha = 0.3f;
            vList2.IsStatic = false; // test (TODO remove line)
            
            // A list may also contain layout panes, the behaviour of 
            // when they are selected must be defined manualy though
            for(int i = 0; i < 3; ++i) {
                ImageItem iiThumb = new ImageItem("images/kevin_sheet", new Rectangle(0, 32, 16, 16));
                TextItem tiInfo = new TextItem(i + ": Lorem ipsum varum esit", "fonts/menu_font1");
                TextItem tiDetail = new TextItem("Sevum: 33.3f, Ralte: 434, Egonger it relum", "fonts/menu_font2");

                // some fun extra stuff (not really good performance wise...)
                // it should technically be possible to keep this going forever
                HList hlInfo = null;
                if(i == 42) { // make this statement always false to disable extra
                    hlInfo = new HList();
                    hlInfo.VisibleRange = 0;
                    for(int j = i; j < 100; ++j) {
                        TextItem ti = new TextItem(j + ": Lorem ipsum varum esit", "fonts/menu_font1");
                        ti.PixelPerSecond = 700;
                        hlInfo.add(ti);
                    }
                }

                HPane hPane;
                if(hlInfo == null) hPane = new HPane(iiThumb, new VPane(tiInfo, tiDetail));
                else hPane = new HPane(iiThumb, new VPane(hlInfo, tiDetail));

                hPane.PercentWidth = 50;
                hPane.Color = new Color(rng.Next(256), rng.Next(256), rng.Next(256));
                iiThumb.DefaultScale = 4;

                // We can set a secondary color for when an item is selected
                iiThumb.SecondaryColor = Color.White;
                tiInfo.SecondaryColor = Color.Yellow;
                tiDetail.SecondaryColor = Color.Yellow;

                vList2.add(hPane);                
            }

            // This is the proper way to register a handler
            vList2.SelectedEvent += (sender, args) => {
                HPane hPane = (HPane)args.SelectedItem;
                ImageItem iiThumb = (ImageItem)hPane.Children[0];
                ScreenComponent infoItem = ((Container)hPane.Children[1]).Children[0];
                if(infoItem is TextItem) ((TextItem)infoItem).IsSelected = true;
                if(infoItem is MenuList) ((MenuList)infoItem).IsFocused = true;

                // Here we define what should should happen if this
                // pane becomes selected, in this example we will
                // select the thumbail and the info text.
                iiThumb.IsSelected = true;
            };

            // We also have to define the behaviour of when
            // this pane becomes deselected
            vList2.DeselectedEvent += (sender, args) => {
                HPane hPane = (HPane)args.SelectedItem;
                ImageItem iiThumb = (ImageItem)hPane.Children[0];
                ScreenComponent infoItem = ((Container)hPane.Children[1]).Children[0];
                if(infoItem is TextItem) ((TextItem)infoItem).IsSelected = false;
                if(infoItem is MenuList) ((MenuList)infoItem).IsFocused = false;
                iiThumb.IsSelected = false;
            };

            // And the handler for when an entry was clicked
            vList2.ActionEvent += (sender, args) => {
                args.SelectedItem.Color = new Color(rng.Next(256), rng.Next(256), rng.Next(256));
                // if(args.SelectedIndex == 1) vList2.IsStatic = !vList2.IsStatic; // TODO fix static change bug
            };

            // Defining handler for switching between the lists, we use
            // the KeyPressedEvent here, checking if a "next key" has been
            // pressed while we are already at the last index, though it
            // would probably be achievable with the SelectedEvent aswell
            int lastSelected = -1;
            vList.KeyPressedEvent += (sender, args) => {
                if(args.KeyboardState.Value.IsKeyDown(Keys.D2)) {
                    if(vList.SelectedIndex == lastSelected) {
                        vList.IsFocused = false;
                        ((MenuList)vList.SelectedItem).IsFocused = false;
                        vList2.IsFocused = true;
                        vList2.select(0);
                    }

                    lastSelected = vList.SelectedIndex;
                }
            };

            // Checking for a "prev key" press while the selected index is 0
            vList2.KeyPressedEvent += (sender, args) => {
                if(args.KeyboardState.Value.IsKeyDown(Keys.D1)) {
                    HPane hPane = (HPane)vList2.SelectedItem;
                    ImageItem iiThumb = (ImageItem)hPane.Children[0];
                    ScreenComponent infoItem = ((Container)hPane.Children[1]).Children[0];
                    if(infoItem is TextItem) ((TextItem)infoItem).IsSelected = false;
                    if(infoItem is MenuList) ((MenuList)infoItem).IsFocused = false;
                    iiThumb.IsSelected = false;
                    vList.IsFocused = true;
                    vList2.IsFocused = false;
                    vList.select(vList.Children.Count-1);
                }
            };

            // reselect first items since new ones have been added since creation
            vList.select(0);
            // vList2.select(0);

            // It is adviced that a layout panes percent width and height
            // are greater than 0 if this is the case for (any of) its children
            // if not the sizing logic may not work as expected
            VPane vPaneMain = new VPane(vList, vList2);
            vPaneMain.PercentWidth = 100;
            vPaneMain.PercentHeight = 100;

            // The actual screen with its main container beeing a stack pane,
            // the main containers percent width and height attributes will be
            // set to 100 within the XtendedScreen ctor. Components within a
            // stack pane will be centered by default.
            XtendedScreen mainScreen = new XtendedScreen(new StackPane(vPaneMain));

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

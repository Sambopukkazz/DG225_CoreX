using Gum;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Gum.GueDeriving;
using Gum.Input;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using RenderingLibrary.Graphics;
using System.Collections.Generic;

namespace Imaginophobia {
    public class CreditsScreen : Screen {
        private Panel _root;

        private static List<(string Text, bool IsHeading)> CreditLines = new List<(string Text, bool IsHeading)> {
            ("Direction/Production", true),
            ("Thinanthanat Pananon", false),
            ("\nArt Direction/Character Design", true),
            ("Nadtawat Temmuang", false),
            ("\nLead Programming/Programming", true),
            ("Techin Jarounsing", false),
            ("\nVisual Direction", true),
            ("Thinanthanat Pananon", false),
            ("\nGame Planning", true),
            ("Techin Jarounsing", false),
            ("Thinanthanat Pananon", false),
            ("Nadtawat Temmuang", false),
            ("Pornpavit Thanakitrungroj", false),
            ("\nAssistant Production", true),
            ("Techin Jarounsing", false),
            ("\nSound Design", true),
            ("Thinanthanat Pananon", false),
            ("Pornpavit Thanakitrungroj", false),
            ("\nAudio Mastering", true),
            ("Techin Jarounsing", false),
            ("\nStory Writer", true),
            ("Nadtawat Temmuang", false),
            ("\nImaginophobia Logo", true),
            ("Thinanthanat Pananon", false),
            ("\nSpecial Thanks", true),
            ("AJ Patison Palee", false),
            ("AJ Keattikorn Samarnggoon", false),
            ("AJ Noppon Wongta", false),
            ("AJ Supara Grudpan", false),
            ("and you!", false)
        };

        public CreditsScreen() {
            _root = new Panel();
            _root.Dock(Dock.Fill);

            ContainerRuntime canvas = new ContainerRuntime();
            canvas.Width = Layout.ScreenWidth;
            canvas.Height = Layout.ScreenHeight;
            _root.AddChild(canvas);

            SpriteRuntime background = new SpriteRuntime();
            background.SourceFileName = "UI/Imaginophobia_bg_placerholder.png";
            canvas.AddChild(background);

            canvas.AddChild(GumUIHelper.CreateTitle("Credits"));
            canvas.AddChild(GumUIHelper.CreateTextButton("Back", Layout.BackButtonFontScale, Layout.BackButtonX, Layout.BackButtonY, Layout.BackButtonSize, Layout.BackButtonSize, Back));
            canvas.AddChild(GumUIHelper.CreateDimPanel());
            canvas.AddChild(CreateCreditsScroll());

            // Dummy 1x1 ScrollViewer - required, not spare. While the screen owns only one
            // ScrollViewer, disposing the screen leaves Gum unable to draw any screen shown
            // afterwards (everything renders black, silently, with no exception). Keep it on
            // any screen that owns a ScrollViewer.
            ScrollViewer dummy = new ScrollViewer();
            dummy.X = 0;
            dummy.Y = 0;
            dummy.Width = 1;
            dummy.Height = 1;
            dummy.InnerPanel.StackSpacing = 2;
            canvas.AddChild(dummy);

            _root.AddToRoot();
        }

        public override void Update(GameTime gameTime) {
            if (GumService.Default.Keyboard.KeyPushed(Keys.Escape)) {
                Back();
            }
        }

        public override void Draw(GameTime gameTime) {
        }

        public override void OnActivated() {
            _root.IsVisible = true;
        }

        public override void OnDeactivated() {
            _root.IsVisible = DrawWhenInactive;
        }

        public override void Dispose() {
            _root.RemoveFromRoot();
            base.Dispose();
        }

        private ScrollViewer CreateCreditsScroll() {
            ScrollViewer scrollViewer = new ScrollViewer();
            ScrollViewerVisual visual = (ScrollViewerVisual)scrollViewer.Visual;
            ScrollBarVisual scrollBar = (ScrollBarVisual)visual.VerticalScrollBarInstance;
            ButtonVisual thumb = (ButtonVisual)scrollBar.ThumbInstance;

            scrollViewer.X = Layout.PanelX;
            scrollViewer.Y = Layout.PanelY;
            scrollViewer.Width = Layout.PanelWidth;
            scrollViewer.Height = Layout.PanelHeight;
            scrollViewer.InnerPanel.StackSpacing = 2;

            visual.BackgroundColor = new Color(0, 0, 0, 0);
            scrollBar.UpButtonInstance.Visible = false;
            scrollBar.DownButtonInstance.Visible = false;

            thumb.XOrigin = HorizontalAlignment.Center;
            thumb.YOrigin = VerticalAlignment.Center;
            thumb.Width = 42;
            thumb.Height = 42;
            thumb.WidthUnits = DimensionUnitType.Absolute;
            thumb.HeightUnits = DimensionUnitType.Absolute;
            thumb.Background.XOrigin = HorizontalAlignment.Center;
            thumb.Background.YOrigin = VerticalAlignment.Center;
            thumb.Background.Width = 42;
            thumb.Background.Height = 42;
            thumb.Background.WidthUnits = DimensionUnitType.Absolute;
            thumb.Background.HeightUnits = DimensionUnitType.Absolute;
            thumb.Background.Color = new Color(0, 0, 0, 0);

            SpriteRuntime thumbImage = new SpriteRuntime();
            thumbImage.SourceFileName = "UI/slider_thumbinstance_placeholder.png";
            thumb.Background.Children.Insert(0, thumbImage);

            thumb.States.Enabled.Clear();
            thumb.States.Highlighted.Clear();
            thumb.States.Pushed.Clear();
            thumb.States.Disabled.Clear();
            thumb.States.Enabled.Apply = () => {
                thumbImage.Color = new Color(255, 255, 255);
            };
            thumb.States.Highlighted.Apply = () => {
                thumbImage.Color = new Color(230, 230, 230);
            };
            thumb.States.Pushed.Apply = () => {
                thumbImage.Color = new Color(215, 215, 215);
            };

            scrollViewer.ScrollChanged += (_, _) => {
                float maximum = (float)scrollViewer.VerticalScrollBarMaximum;
                float progress = 0f;
                if (maximum > 0) {
                    progress = (float)scrollViewer.VerticalScrollBarValue / maximum;
                }
                thumb.Background.Rotation = progress * 720f;
                //AudioManager.Instance.PlaySliderStep();
            };

            scrollBar.TrackInstance.Texture = null;

            SpriteRuntime pipe = new SpriteRuntime();
            pipe.SourceFileName = "UI/slider_placeholder.png";
            pipe.Width = 580;
            pipe.Height = 30;
            pipe.WidthUnits = DimensionUnitType.Absolute;
            pipe.HeightUnits = DimensionUnitType.Absolute;
            pipe.XOrigin = HorizontalAlignment.Center;
            pipe.YOrigin = VerticalAlignment.Center;
            pipe.X = 12;
            pipe.Y = 305;
            pipe.Rotation = 90;
            scrollBar.TrackInstance.Children.Add(pipe);

            foreach ((string Text, bool IsHeading) line in CreditLines) {
                scrollViewer.InnerPanel.AddChild(CreateCreditText(line.Text, line.IsHeading));
            }

            return scrollViewer;
        }

        private TextRuntime CreateCreditText(string text, bool isHeading) {
            float fontScale = 1.5f;
            float width = 1000;
            float height = 60;
            if (isHeading) {
                fontScale = 2.5f;
                width = 1480;
                height = 40;
            }

            TextRuntime credit = new TextRuntime();
            credit.Text = text;
            credit.Font = "generationfonting";
            credit.FontScale = fontScale;
            credit.Width = width;
            credit.Height = height;
            credit.X = 40;
            credit.VerticalAlignment = VerticalAlignment.Center;
            credit.HasDropshadow = true;
            credit.OutlineThickness = 1;
            return credit;
        }

        private void Back() {
            MainGame.ScreenManager.CloseScreen();
        }
    }
}

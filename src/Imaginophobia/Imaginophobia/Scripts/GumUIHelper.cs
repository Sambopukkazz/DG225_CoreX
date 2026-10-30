using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Gum.GueDeriving;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RenderingLibrary.Content;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace Imaginophobia {
    // Builds every Gum control the screens use - text buttons, the pause button, sliders,
    // value labels and the dim backdrop panel - so they all share one styling (the
    // generationfonting font, dropshadow + outline text, transparent button background).
    public static class GumUIHelper {
        public static TextRuntime CreateTitle(string text) {
            TextRuntime title = new TextRuntime();
            title.Text = text;
            title.Width = Layout.TitleWidth;
            title.Height = Layout.TitleHeight;
            title.X = Layout.TitleX;
            title.Y = Layout.TitleY;
            title.Font = "generationfonting";
            title.FontScale = Layout.TitleFontScale;
            title.WidthUnits = DimensionUnitType.Absolute;
            title.HeightUnits = DimensionUnitType.Absolute;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.VerticalAlignment = VerticalAlignment.Center;
            title.HasDropshadow = true;
            title.OutlineThickness = 1;
            return title;
        }

        // Text label at a fixed position and size, centered on both axes.
        public static TextRuntime CreateText(string text, float fontScale, int x, int y, int width, int height, bool hasDropShadow = true, int outlineThickness = 1) {
            TextRuntime label = new TextRuntime();
            label.Text = text;
            label.Font = "generationfonting";
            label.FontScale = fontScale;
            label.X = x;
            label.Y = y;
            label.Width = width;
            label.Height = height;
            label.WidthUnits = DimensionUnitType.Absolute;
            label.HeightUnits = DimensionUnitType.Absolute;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HasDropshadow = hasDropShadow;
            label.OutlineThickness = outlineThickness;
            return label;
        }

        // Text button with a transparent background and three visual states:
        //   enabled     : label at rest
        //   pushed      : label pushed down 10px, plays the click sound
        //   highlighted : label dimmed to 230, plays the hover sound
        // Pass enabled = false for the non-interactive menu arrows and the Index button.
        public static Button CreateTextButton(string text, float fontScale, int x, int y, int width, int height, Action onClick, bool enabled = true) {
            Button button = new Button();
            ButtonVisual visual = (ButtonVisual)button.Visual;
            TextRuntime label = visual.TextInstance;

            label.Text = text;
            label.Font = "generationfonting";
            label.FontScale = fontScale;
            label.HasDropshadow = true;
            label.OutlineThickness = 1;

            visual.BackgroundColor = new Color(255, 255, 255, 0);

            visual.States.Enabled.Clear();
            visual.States.Enabled.Apply = () => {
                label.Y = 0;
                label.Color = new Color(255, 255, 255);
            };

            visual.States.Pushed.Clear();
            visual.States.Pushed.Apply = () => {
                label.Y = label.Y + 10;
                //AudioManager.Instance.PlayButtonPush();
            };

            visual.States.Highlighted.Clear();
            visual.States.Highlighted.Apply = () => {
                label.Color = new Color(230, 230, 230);
                //AudioManager.Instance.PlayButtonHighlight();
            };

            button.Width = width;
            button.Height = height;
            button.WidthUnits = DimensionUnitType.Absolute;
            button.HeightUnits = DimensionUnitType.Absolute;
            button.X = x;
            button.Y = y;
            if (onClick != null) {
                button.Click += (_, _) => onClick();
            }
            button.IsEnabled = enabled;

            return button;
        }

        public static Button CreatePauseButton(Action onClick) {
            Button button = new Button();
            ButtonVisual visual = (ButtonVisual)button.Visual;
            TextRuntime label = visual.TextInstance;

            visual.Background.Parent = null;
            SpriteRuntime image = new SpriteRuntime();
            image.SourceFileName = "UI/sprite_button_pause.png";
            visual.Children.Insert(0, image);

            label.Text = "Settings";
            label.Font = "generationfonting";
            label.FontScale = 0.5f;
            label.HasDropshadow = true;
            label.OutlineThickness = 2;

            button.Width = 60;
            button.Height = 60;
            button.WidthUnits = DimensionUnitType.Absolute;
            button.HeightUnits = DimensionUnitType.Absolute;
            button.X = 1820;
            button.Y = 40;
            button.Click += (_, _) => onClick();

            visual.States.Enabled.Clear();
            visual.States.Enabled.Apply = () => {
                button.Y = 40;
                image.Color = new Color(255, 255, 255);
            };

            visual.States.Highlighted.Clear();
            visual.States.Highlighted.Apply = () => {
                image.Color = new Color(230, 230, 230);
                //AudioManager.Instance.PlayButtonHighlight();
            };

            visual.States.Pushed.Clear();
            visual.States.Pushed.Apply = () => {
                button.Y = 42.5f;
                //AudioManager.Instance.PlayButtonPush();
            };

            return button;
        }

        public static Slider CreateSlider(int x, int y) {
            Slider slider = new Slider();
            slider.Width = Layout.SliderWidth;
            slider.X = x;
            slider.Y = y;
            slider.Minimum = 0;
            slider.Maximum = 100;

            SliderVisual visual = (SliderVisual)slider.Visual;
            visual.TrackBackground.Texture = MainGame.Content.Load<Texture2D>("UI/slider_placeholder");
            visual.TrackBackground.TextureAddress = TextureAddress.EntireTexture;
            visual.TrackBackground.Height = 25;
            visual.TrackBackground.Color = Color.White;

            ButtonVisual thumb = (ButtonVisual)visual.ThumbInstance;
            thumb.Width = 42;
            thumb.Height = 42;
            thumb.WidthUnits = DimensionUnitType.Absolute;
            thumb.HeightUnits = DimensionUnitType.Absolute;
            thumb.Background.Width = 42;
            thumb.Background.Height = 42;
            thumb.Background.WidthUnits = DimensionUnitType.Absolute;
            thumb.Background.HeightUnits = DimensionUnitType.Absolute;
            thumb.Background.Parent = null;

            SpriteRuntime thumbImage = new SpriteRuntime();
            thumbImage.SourceFileName = "UI/slider_thumbinstance_placeholder.png";
            thumb.Children.Insert(0, thumbImage);

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

            slider.ValueChanged += (_, _) => {
                thumb.Rotation = (float)(slider.Value * 3.6);
                // note: this also fires for the programmatic initial Value set, so gate it
                // on a first-set flag if the step sound proves noisy on screen entry
                //AudioManager.Instance.PlaySliderStep();
            };

            return slider;
        }

        public static Label CreateValueLabel(int x, int y, float fontScale = 1) {
            Label label = new Label();
            label.X = x;
            label.Y = y;

            TextRuntime text = (TextRuntime)label.TextComponent;
            text.Font = "generationfonting";
            text.FontScale = fontScale;
            text.HasDropshadow = true;
            text.OutlineThickness = 1;

            return label;
        }

        public static RectangleRuntime CreateDimPanel() {
            RectangleRuntime panel = new RectangleRuntime();
            panel.IsFilled = true;
            panel.FillColor = new Color(0, 0, 0, 100);
            panel.StrokeColor = new Color(0, 0, 0, 0);
            panel.X = Layout.PanelX;
            panel.Y = Layout.PanelY;
            panel.Width = Layout.PanelWidth;
            panel.Height = Layout.PanelHeight;
            panel.CornerRadius = Layout.PanelCornerRadius;
            panel.HasDropshadow = true;
            panel.DropshadowOffsetX = 0;
            panel.DropshadowOffsetY = 0;
            panel.DropshadowAlpha = 255;
            panel.DropshadowBlur = 100;
            return panel;
        }

        // Dim panel + "Settings" title + two labelled sliders, built once and reused by
        // SettingsScreen and the pause overlay. No action button - callers add their own.
        public static void AddSettingsPanel(ContainerRuntime parent) {
            parent.AddChild(CreateDimPanel());
            parent.AddChild(CreateTitle("Settings"));

            Slider backgroundSoundSlider = CreateSlider(Layout.SliderX, Layout.AmbienceRowY);
            parent.AddChild(backgroundSoundSlider);
            Label backgroundSoundValue = CreateValueLabel(Layout.SliderValueX, Layout.AmbienceRowY);
            parent.AddChild(backgroundSoundValue);
            TextRuntime ambienceLabel = CreateText("Ambience Volume", Layout.MenuFontScale, Layout.SliderLabelX, Layout.AmbienceLabelY, Layout.SliderLabelWidth, Layout.SliderLabelHeight);
            ambienceLabel.VerticalAlignment = VerticalAlignment.Top;
            parent.AddChild(ambienceLabel);

            Slider sfxSlider = CreateSlider(Layout.SliderX, Layout.SfxRowY);
            parent.AddChild(sfxSlider);
            Label sfxValue = CreateValueLabel(Layout.SliderValueX, Layout.SfxRowY);
            parent.AddChild(sfxValue);
            TextRuntime sfxLabel = CreateText("SFX Volume", Layout.MenuFontScale, Layout.SliderLabelX, Layout.SfxLabelY, Layout.SliderLabelWidth, Layout.SliderLabelHeight);
            sfxLabel.VerticalAlignment = VerticalAlignment.Top;
            parent.AddChild(sfxLabel);

            backgroundSoundSlider.Value = AudioManager.Instance.BgsVolume * 100f;
            backgroundSoundValue.Text = backgroundSoundSlider.Value.ToString();
            backgroundSoundSlider.ValueChanged += (_, _) => {
                AudioManager.Instance.BgsVolume = (float)(backgroundSoundSlider.Value / 100f);
                backgroundSoundValue.Text = backgroundSoundSlider.Value.ToString("N0");
            };

            sfxSlider.Value = AudioManager.Instance.SoundEffectVolume * 100f;
            sfxValue.Text = sfxSlider.Value.ToString();
            sfxSlider.ValueChanged += (_, _) => {
                AudioManager.Instance.SoundEffectVolume = (float)(sfxSlider.Value / 100f);
                sfxValue.Text = sfxSlider.Value.ToString("N0");
            };
        }
    }
}

using Microsoft.Xna.Framework;

namespace Imaginophobia {
    // Design-space constants for every screen: screen size, the shared backdrop panel, the
    // title block, the menu column, the settings rows and the pause overlay's return button.
    // Screens read these instead of scattering literals.
    public static class Layout {
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;

        // Shared dim backdrop panel used by Settings and the pause overlay.
        public const int PanelX = 180;
        public const int PanelY = 300;
        public const int PanelWidth = 1500;
        public const int PanelHeight = 660;
        public const int PanelCornerRadius = 15;

        // Title block (menu, settings, credits).
        public const int TitleX = 540;
        public const int TitleY = 120;
        public const int TitleWidth = 840;
        public const int TitleHeight = 120;
        public const float TitleFontScale = 4;

        // Centered menu column.
        public const int MenuButtonWidth = 240;
        public const int MenuButtonHeight = 80;
        public const int MenuButtonX = 840;
        public const float MenuFontScale = 2;
        public const int MenuIndexY = 520;
        public const int MenuSettingsY = 640;
        public const int MenuCreditsY = 760;
        public const int MenuExitY = 880;

        // Start ("Shift 1") button - larger than the rest.
        public const int StartButtonX = 780;
        public const int StartButtonY = 360;
        public const int StartButtonWidth = 360;
        public const int StartButtonHeight = 120;
        public const float StartButtonFontScale = 3;

        // Menu navigation arrows (disabled).
        public const int LeftArrowX = 660;
        public const int RightArrowX = 1140;
        public const int ArrowY = 360;
        public const int ArrowSize = 120;
        public const float ArrowFontScale = 3;

        // Top-left back button.
        public const int BackButtonSize = 120;
        public const int BackButtonX = 120;
        public const int BackButtonY = 120;
        public const float BackButtonFontScale = 2;

        // Settings rows.
        public const int SliderX = 1080;
        public const int SliderWidth = 400;
        public const int SliderValueX = 1520;
        public const int SliderLabelX = 260;
        public const int SliderLabelWidth = 500;
        public const int SliderLabelHeight = 120;
        public const int AmbienceRowY = 365;
        public const int SfxRowY = 490;
        public const int AmbienceLabelY = 350;
        public const int SfxLabelY = 470;

        // Return-to-menu button (pause overlay).
        public const int ActionButtonX = 760;
        public const int ActionButtonY = 820;
        public const int ActionButtonWidth = 400;
        public const int ActionButtonHeight = 80;

        public static Rectangle FullScreen => new Rectangle(0, 0, ScreenWidth, ScreenHeight);
        public static Rectangle Panel => new Rectangle(PanelX, PanelY, PanelWidth, PanelHeight);
    }
}

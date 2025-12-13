using MudBlazor;

namespace SkillShareMap.Shared
{
    public static class AppTheme
    {
        public static MudTheme Default = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                // greyish blue
                Primary = "#4a75b0",

                // dark black
                Secondary = "#1B1B1B",

                // deep greyish blue
                Tertiary = "#7B9CC9",

                // deep blue
                AppbarBackground = "#1976d2",

                // white surface
                Surface = "#FFFFFF"
            },

            Typography = new Typography()
            {
                Default = new DefaultTypography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" }
                },
                H1 = new H1Typography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "700"
                },
                H2 = new H2Typography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "700"
                },
                H3 = new H3Typography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "600"
                },
                H4 = new H4Typography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "600"
                },
                H5 = new H5Typography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "600"
                },
                H6 = new H6Typography() // 20px
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "600"
                },
                Button = new ButtonTypography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontWeight = "600"
                },
                Body1 = new Body1Typography() // 16px
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" }
                },
                Body2 = new Body2Typography() // 14px
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" }
                },
                Subtitle1 = new Subtitle1Typography() // 16px
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" }
                },
                Subtitle2 = new Subtitle2Typography() // 14px
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" }
                },
                Caption = new CaptionTypography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontSize = "12px",
                    LineHeight = "1.5",
                    FontWeight = "500"
                },
                Overline = new OverlineTypography()
                {
                    FontFamily = new[] { "IBM Plex Sans", "Mona Sans", "sans-serif" },
                    FontSize = "10px",
                    LineHeight = "1.5",
                    LetterSpacing = "0",
                    TextTransform = "none"
                }

            }
        };
    }
}
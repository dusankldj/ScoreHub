namespace MAUI.Helpers
{
    public enum IconFontFamily
    {
        FontAwesome,
        MaterialDesignIcons
    }

    public record SportIcon(string Glyph, IconFontFamily Font)
    {
        public string FontFamilyName => Font == IconFontFamily.MaterialDesignIcons ? "MDI" : "FA7Solid";
    }
}

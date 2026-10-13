using MAUI.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services
{
    public class SportMenuItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string IconGlyph { get; set; }
        public string IconFontFamily { get; set; } = "FA7Solid"; // "FA7Solid" ili "MDI"
    }
}

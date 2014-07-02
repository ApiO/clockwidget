using System;

namespace ClockWidget
{
    [Serializable]
    public class Configuration
    {
        public bool Topmost { get; set; }
        public bool Lock { get; set; }
        public double FontSize { get; set; }
        public double Opacity { get; set; }
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }

        public Configuration()
        {
            FontSize = 64.0f;
            Opacity = 0.5f;
            BackgroundColor = "#00FFFFFF";
            FontColor = "#FFFFFFFF";
            Lock = false;
        }

        public Configuration Clone()
        {
            return new Configuration()
            {
                 FontSize = this.FontSize,
                 Opacity = this.Opacity,
                 BackgroundColor = this.BackgroundColor,
                 FontColor = this.FontColor,
                 Lock = this.Lock,
                 Topmost = this.Topmost
            };
        }
    }
}

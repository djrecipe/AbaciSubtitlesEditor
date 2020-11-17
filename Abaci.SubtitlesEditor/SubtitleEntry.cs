using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    class SubtitleEntry
    {
        public string Content { get; set; }
        public string Label { get; set; }
        public TimeSpan StartTime { get;set; }
        public TimeSpan EndTime { get; set; }
        public override string ToString()
        {
            string text = $"{this.Label}\r\n{this.StartTime} --> {this.EndTime}\r\n{this.Content}";
            return text;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    public class SubtitleEntryCollection : ObservableCollection<SubtitleEntry>
    {
        public TimeSpan EndTime
        {
            get
            {
                return this.Items.Select(i => i.EndTime).Max();
            }
        }

        public override string ToString()
        {
            string text = string.Join("\r\n\r\n", this.Select(s => s.ToString()));
            return text;
        }
    }
}

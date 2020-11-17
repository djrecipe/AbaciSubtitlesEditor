using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    class SubtitleEntryCollection : ObservableCollection<SubtitleEntry>
    {
        public TimeSpan EndTime
        {
            get
            {
                return this.Items.Select(i => i.EndTime).Max();
            }
        }
    }
}

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
        public void ApplyOffset(TimeSpan offset)
        {
            foreach(SubtitleEntry subtitle in this)
            {
                subtitle.StartTime += offset;
                subtitle.EndTime += offset;
            }
            return;
        }
        public override string ToString()
        {
            string text = string.Join("\r\n\r\n", this.Select(s => s.ToString()));
            return text;
        }
        public void Translate(ITranslationProvider translator, string language)
        {
            foreach(SubtitleEntry subtitle in this)
            {
                subtitle.Content = translator.Translate(subtitle.Content, language);
            }
        }
    }
}

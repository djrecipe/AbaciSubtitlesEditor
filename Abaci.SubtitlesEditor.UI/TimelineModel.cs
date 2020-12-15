using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

namespace Abaci.SubtitlesEditor.UI
{
    public class TimelineModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public SubtitleEntry SelectedSubtitle
        {
            get
            {
                return this._Subtitles?.FirstOrDefault(s => s.IsSelected);
            }
            set
            {
                SubtitleEntry entry = this._Subtitles?.FirstOrDefault(s => s.StartTime == value.StartTime && s.EndTime == value.EndTime);
                if(entry != null)
                    entry.IsSelected = true;
                return;
            }
        }
        private SubtitleEntryCollection _Subtitles = new SubtitleEntryCollection();
        public SubtitleEntryCollection Subtitles
        {
            get
            {
                return this._Subtitles;
            }
            set
            {
                this._Subtitles = value;
                this.InvokePropertyChangedEvent(nameof(this.Subtitles));
                return;
            }
        }
        private void InvokePropertyChangedEvent(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }
    }
}

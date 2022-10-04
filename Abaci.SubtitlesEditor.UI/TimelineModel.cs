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

        public string CurrentTimeString
        {
            get
            {
                return this.CurrentTime.ToString("g");
            }
        }

        private TimeSpan _CurrentTime = default(TimeSpan);
        public TimeSpan CurrentTime
        {
            get
            {
                return this._CurrentTime;
            }
            set
            {
                this._CurrentTime = value;
                this.InvokePropertyChangedEvent(nameof(this.CurrentTime));
                this.InvokePropertyChangedEvent(nameof(this.CurrentTimeString));
            }
        }
        private double _MinSliderValue = 0.0;
        public double MinSliderValue
        {
            get
            {
                return this._MinSliderValue;
            }
            set
            {
                this._MinSliderValue = value;
                this.InvokePropertyChangedEvent(nameof(this.MinSliderValue));
                return;
            }
        }
        private double _MaxSliderValue = 1000000.0;
        public double MaxSliderValue
        {
            get
            {
                return this._MaxSliderValue;
            }
            set
            {
                this._MaxSliderValue = value;
                this.InvokePropertyChangedEvent(nameof(this.MaxSliderValue));
                return;
            }
        }
        private double _CurrentSliderValue = 1000.0;
        public double CurrentSliderValue
        {
            get
            {
                return this._CurrentSliderValue;
            }
            set
            {
                this._CurrentSliderValue = value;
                this.CurrentTime = TimeSpan.FromMilliseconds(this.CurrentSliderValue);
                this.InvokePropertyChangedEvent(nameof(this.CurrentSliderValue));
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

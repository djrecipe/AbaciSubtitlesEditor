using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    public class SubtitleEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _Content = null;
        public string Content
        {
            get
            {
                return this._Content;
            }
            set
            {
                this._Content = value;
                this.InvokePropertyChangedEvent(nameof(Content));
            }
        }
        private bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return this._IsSelected;
            }
            set
            {
                this._IsSelected = value;
                this.InvokePropertyChangedEvent(nameof(IsSelected));
            }
        }
        private string _Label = null;
        public string Label
        {
            get
            {
                return this._Label;
            }
            set
            {
                this._Label = value;
                this.InvokePropertyChangedEvent(nameof(Content));
            }
        }
        private TimeSpan _StartTime = default(TimeSpan);
        public TimeSpan StartTime
        {
            get
            {
                return this._StartTime;
            }
            set
            {
                this._StartTime = value;
                this.InvokePropertyChangedEvent(nameof(StartTime));
            }
        }
        private TimeSpan _EndTime = default(TimeSpan);
        public TimeSpan EndTime
        {
            get
            {
                return this._EndTime;
            }
            set
            {
                this._EndTime = value;
                this.InvokePropertyChangedEvent(nameof(EndTime));
            }
        }


        public override string ToString()
        {
            string text = $"{this.Label}\r\n{this.StartTime.ToString(@"hh\:mm\:ss\.fff")} --> {this.EndTime.ToString(@"hh\:mm\:ss\.fff")}\r\n{this.Content}";
            text = text.Trim('\n', '\r', ' ').Trim('\n', '\r', ' ');
            return text;
        }
        private void InvokePropertyChangedEvent(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }
    }
}

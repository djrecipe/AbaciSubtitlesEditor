using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Abaci.SubtitlesEditor;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.IO;

namespace Abaci.SubtitlesEditor.UI
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        private ITranslationProvider translator = null;
        private readonly SubtitleEntryFactory factory = new SubtitleEntryFactory();
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CommandApplyOffset { get; private set; }
        public ICommand CommandOpenFile { get; private set; }
        public ICommand CommandSaveFile { get; private set; }
        public ICommand CommandTranslate { get; private set; }
        private TimeSpan _Offset = default(TimeSpan);
        public TimeSpan Offset
        {
            get
            {
                return this._Offset;
            }
            set
            {
                this._Offset = value;
                this.InvokePropertyChangedEvent(nameof(this.Offset));
                this.OffsetString = this.Offset.ToString();
            }
        }
        public string OffsetString
        {
            get
            {
                return this.Offset.ToString();
            }
            set
            {
                TimeSpan result = default(TimeSpan);
                TimeSpan.TryParse(value, out result);
                this._Offset = result;
                this.InvokePropertyChangedEvent(nameof(this.OffsetString));
            }
        }
        private string _RawText = null;
        public string RawText
        {
            get
            {
                return this._RawText;
            }
            set
            {
                this._RawText = value;
                this.InvokePropertyChangedEvent(nameof(this.RawText));
            }
        }
        private int _SelectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get
            {
                return this._SelectedTabIndex;
            }
            set
            {
                this._SelectedTabIndex = value;
                this.UpdateData();
                this.InvokePropertyChangedEvent(nameof(this.SelectedTabIndex));
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
        private string _TargetLanguage = "Thai";
        public string TargetLanguage
        {
            get
            {
                return this._TargetLanguage.ToString();
            }
            set
            {
                this._TargetLanguage = value;
                this.InvokePropertyChangedEvent(nameof(this.TargetLanguage));
            }
        }
        internal MainWindowModel()
        {
            //this.translator = new GoogleTranslationProvider();
            this.CommandApplyOffset = new RelayCommand(this.ExecuteCommandApplyOffset, this.CanExecuteCommandApplyOffset);
            this.CommandOpenFile = new RelayCommand(this.ExecuteCommandOpenFile, this.CanExecuteCommandOpenFile);
            this.CommandSaveFile = new RelayCommand(this.ExecuteCommandSaveFile, this.CanExecuteCommandSaveFile);
            this.CommandTranslate = new RelayCommand(this.ExecuteCommandTranslate, this.CanExecuteCommandTranslate);
        }
        private bool CanExecuteCommandApplyOffset()
        {
            return true;
        }
        private bool CanExecuteCommandOpenFile()
        {
            return true;
        }
        private bool CanExecuteCommandSaveFile()
        {
            return true;
        }
        private bool CanExecuteCommandTranslate()
        {
            return true;
        }
        private void ExecuteCommandApplyOffset()
        {
            this.Subtitles.ApplyOffset(this.Offset);
        }
        private void ExecuteCommandOpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.Subtitles = this.factory.CreateFromFile(dialog.FileName);
                this.RawText = this.Subtitles.ToString();
            }
        }
        private void ExecuteCommandSaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Subtitles|*.srt";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.factory.Save(this.Subtitles, dialog.FileName);
            }
        }
        private void ExecuteCommandTranslate()
        {
            this.Subtitles.Translate(this.translator, this.TargetLanguage);
        }
        private void UpdateData()
        {
            switch(this.SelectedTabIndex)
            {
                case 0:
                    this.RawText = this.Subtitles?.ToString();
                    return;
                case 1:
                    this.Subtitles = this.factory.CreateFromText(this.RawText);
                    break;
                case 2:
                    this.Subtitles = this.factory.CreateFromText(this.RawText);
                    break;
            }
        }
        private void InvokePropertyChangedEvent(string name)
        {
            if(this.PropertyChanged!=null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }
    }
}

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
        private readonly SubtitleEntryFactory factory = new SubtitleEntryFactory();
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CommandOpenFile { get; private set; }
        public ICommand CommandSaveFile { get; private set; }
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
        internal MainWindowModel()
        {
            this.CommandOpenFile = new RelayCommand(this.ExecuteCommandOpenFile, this.CanExecuteCommandOpenFile);
            this.CommandSaveFile = new RelayCommand(this.ExecuteCommandSaveFile, this.CanExecuteCommandSaveFile);
        }
        private bool CanExecuteCommandOpenFile()
        {
            return true;
        }
        private bool CanExecuteCommandSaveFile()
        {
            return true;
        }
        private void ExecuteCommandOpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.RawText = File.ReadAllText(dialog.FileName);
                this.Subtitles = this.factory.CreateFromFile(dialog.FileName);
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
        private void UpdateData()
        {
            switch(this.SelectedTabIndex)
            {
                case 0:
                    this.RawText = this.Subtitles.ToString();
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

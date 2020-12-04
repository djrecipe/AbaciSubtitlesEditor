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
        private readonly BackgroundWorker workerApplyOffset = new BackgroundWorker();
        private readonly BackgroundWorker workerTranslate = new BackgroundWorker();
        private ITranslationProvider translator = null;
        private readonly SubtitleEntryFactory factory = new SubtitleEntryFactory();
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CommandApplyOffset { get; private set; }
        public ICommand CommandOpenFile { get; private set; }
        public ICommand CommandSaveFile { get; private set; }
        public ICommand CommandTranslate { get; private set; }
        public bool Busy
        {
            get
            {
                return this.workerApplyOffset.IsBusy || this.workerTranslate.IsBusy;
            }
        }
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
                this.OffsetString = this.Offset.ToString(@"hh\:mm\:ss\.fff");
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
            //
            this.workerApplyOffset.DoWork += WorkerApplyOffset_DoWork;
            this.workerApplyOffset.RunWorkerCompleted += WorkerApplyOffset_RunWorkerCompleted;
            this.workerTranslate.DoWork += WorkerTranslate_DoWork;
            this.workerTranslate.RunWorkerCompleted += WorkerTranslate_RunWorkerCompleted;
            //
            //this.translator = new GoogleTranslationProvider();
            this.CommandApplyOffset = new RelayCommand(this.ExecuteCommandApplyOffset, this.CanExecuteCommandApplyOffset);
            this.CommandOpenFile = new RelayCommand(this.ExecuteCommandOpenFile, this.CanExecuteCommandOpenFile);
            this.CommandSaveFile = new RelayCommand(this.ExecuteCommandSaveFile, this.CanExecuteCommandSaveFile);
            this.CommandTranslate = new RelayCommand(this.ExecuteCommandTranslate, this.CanExecuteCommandTranslate);
        }


        private bool CanExecuteCommandApplyOffset()
        {
            return !this.Busy;
        }
        private bool CanExecuteCommandOpenFile()
        {
            return !this.Busy;
        }
        private bool CanExecuteCommandSaveFile()
        {
            return !this.Busy;
        }
        private bool CanExecuteCommandTranslate()
        {
            return !this.Busy;
        }
        private void ExecuteCommandApplyOffset()
        {
            Tuple<SubtitleEntryCollection, TimeSpan, string> values = null;
            switch (this.SelectedTabIndex)
            {
                case 0:
                    values = new Tuple<SubtitleEntryCollection, TimeSpan, string>(null, this.Offset, this.RawText);
                    break;
                case 1:
                case 2:
                    values = new Tuple<SubtitleEntryCollection, TimeSpan, string>(this.Subtitles, this.Offset, null);
                    break;
                default:
                    break;
            }
            this.workerApplyOffset.RunWorkerAsync(values);
            this.InvokePropertyChangedEvent(nameof(this.Busy));
            return;
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
            Tuple<SubtitleEntryCollection, ITranslationProvider, string> values = new Tuple<SubtitleEntryCollection, ITranslationProvider, string>(this.Subtitles, this.translator, this.TargetLanguage);
            this.workerTranslate.RunWorkerAsync(values);
        }
        private void UpdateData()
        {
            switch(this.SelectedTabIndex)
            {
                case 0:
                    this.RawText = this.Subtitles?.ToString();
                    break;
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
        private void WorkerApplyOffset_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<SubtitleEntryCollection, TimeSpan, string> values = e.Argument as Tuple<SubtitleEntryCollection, TimeSpan, string>;
            SubtitleEntryCollection subtitles = new SubtitleEntryCollection();
            if (values.Item3 != null)
            {
                SubtitleEntryFactory temp = new SubtitleEntryFactory();
                subtitles = temp.CreateFromText(values.Item3);
            }
            else
            {
                subtitles = values.Item1;
            }
            subtitles.ApplyOffset(values.Item2);
            Tuple<SubtitleEntryCollection, string> results = new Tuple<SubtitleEntryCollection, string>(subtitles, subtitles.ToString());
            e.Result = results;
            return;
        }
        private void WorkerApplyOffset_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<SubtitleEntryCollection, string> results = e.Result as Tuple<SubtitleEntryCollection, string>;
            switch (this.SelectedTabIndex)
            {
                case 0:
                    this.RawText = results.Item2;
                    break;
                case 1:
                    this.Subtitles = results.Item1;
                    break;
                case 2:
                    this.Subtitles = results.Item1;
                    break;
            }
            this.InvokePropertyChangedEvent(nameof(this.Busy));
        }
        private void WorkerTranslate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
                throw e.Error;
            this.Subtitles = e.Result as SubtitleEntryCollection;
            this.InvokePropertyChangedEvent(nameof(this.Busy));
        }

        private void WorkerTranslate_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<SubtitleEntryCollection, ITranslationProvider, string> values = e.Argument as Tuple<SubtitleEntryCollection, ITranslationProvider, string>;
            Task<SubtitleEntryCollection> task = TranslationProviderTools.ReplaceContent(values.Item1, values.Item2, values.Item3);
            task.Wait();
            e.Result = task.Result;
        }
    }
}

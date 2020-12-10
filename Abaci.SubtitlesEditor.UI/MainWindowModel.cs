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
    internal enum OffsetDirection { Increase, Decrease };
    internal class MainWindowModel : INotifyPropertyChanged
    {
        #region Private Members
        private readonly BackgroundWorker workerApplyOffset = new BackgroundWorker();
        private readonly BackgroundWorker workerTranslate = new BackgroundWorker();
        private ITranslationProvider translator = null;
        private readonly SubtitleEntryFactory factory = new SubtitleEntryFactory();
        #endregion
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        #region Commands
        public ICommand CommandApplyOffset { get; private set; }
        public ICommand CommandOpenFile { get; private set; }
        public ICommand CommandSaveFile { get; private set; }
        public ICommand CommandTranslate { get; private set; }
        #endregion
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
                Properties.Settings.Default.Offset = value;
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
                Properties.Settings.Default.Offset = result;
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
                Properties.Settings.Default.LastTab = value;
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
        #endregion
        #region Instance Methods
        internal MainWindowModel()
        {
            //
            this.workerApplyOffset.DoWork += WorkerApplyOffset_DoWork;
            this.workerApplyOffset.RunWorkerCompleted += WorkerApplyOffset_RunWorkerCompleted;
            this.workerTranslate.DoWork += WorkerTranslate_DoWork;
            this.workerTranslate.RunWorkerCompleted += WorkerTranslate_RunWorkerCompleted;
            //
            //this.translator = new GoogleTranslationProvider();
            this.CommandApplyOffset = new RelayCommand<OffsetDirection>(this.ExecuteCommandApplyOffset, this.CanExecuteCommandApplyOffset);
            this.CommandOpenFile = new RelayCommand(this.ExecuteCommandOpenFile, this.CanExecuteCommandOpenFile);
            this.CommandSaveFile = new RelayCommand(this.ExecuteCommandSaveFile, this.CanExecuteCommandSaveFile);
            this.CommandTranslate = new RelayCommand(this.ExecuteCommandTranslate, this.CanExecuteCommandTranslate);
            return;
        }


        private bool CanExecuteCommandApplyOffset(OffsetDirection direction)
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
        private void ExecuteCommandApplyOffset(OffsetDirection direction)
        {
            try
            {
                TimeSpan offset = this.Offset;
                switch (direction)
                {
                    case OffsetDirection.Decrease:
                        offset = new TimeSpan(this.Offset.Ticks * -1);
                        break;
                    case OffsetDirection.Increase:
                        offset = new TimeSpan(this.Offset.Ticks);
                        break;
                }
                this.ApplyOffset(offset);
                this.InvokePropertyChangedEvent(nameof(this.Busy));
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void ExecuteCommandOpenFile()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    Properties.Settings.Default.LastFilePath = dialog.FileName;
                    this.OpenSubtitlesFile(dialog.FileName);
                }
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void ExecuteCommandSaveFile()
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Subtitles|*.srt";
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    this.SaveSubtitlesToFile(dialog.FileName);
                }
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void ExecuteCommandTranslate()
        {
            try
            {
                this.TranslateSubtitlesContent();
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void ApplyOffset(TimeSpan offset)
        {
            Tuple<SubtitleEntryCollection, TimeSpan, string> values = null;
            switch (this.SelectedTabIndex)
            {
                case 0:
                    values = new Tuple<SubtitleEntryCollection, TimeSpan, string>(null, offset, this.RawText);
                    break;
                case 1:
                case 2:
                    values = new Tuple<SubtitleEntryCollection, TimeSpan, string>(this.Subtitles, offset, null);
                    break;
                default:
                    break;
            }
            this.workerApplyOffset.RunWorkerAsync(values);
            return;
        }
        public void Cleanup()
        {
            try
            {
                this.SaveSettings();
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void LoadSettings()
        {
            // TODO 12/4/20: determine a better way to update data so that it isn't invoked multiple times
            string path = Properties.Settings.Default.LastFilePath;
            if(!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                this.Subtitles = this.factory.CreateFromFile(path);
            this.Offset = Properties.Settings.Default.Offset;
            this.RawText = this.Subtitles?.ToString();
            this.SelectedTabIndex = Properties.Settings.Default.LastTab;
        }
        public void Initialize()
        {
            try
            {
                this.LoadSettings();
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void OpenSubtitlesFile(string path)
        {
            this.Subtitles = this.factory.CreateFromFile(path);
            this.RawText = this.Subtitles.ToString();
            return;
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
        private void SaveSubtitlesToFile(string path)
        {
            this.factory.Save(this.Subtitles, path);
            return;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.Save();
            return;
        }
        private void TranslateSubtitlesContent()
        {
            Tuple<SubtitleEntryCollection, ITranslationProvider, string> values = new Tuple<SubtitleEntryCollection, ITranslationProvider, string>(this.Subtitles, this.translator, this.TargetLanguage);
            this.workerTranslate.RunWorkerAsync(values);
            return;
        }
        #endregion
        #region Instance Event Methods
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
            bool only_selected = subtitles.Any(s => s.IsSelected); // if any items are selected, only offset selected items
            subtitles.ApplyOffset(values.Item2, only_selected);
            Tuple<SubtitleEntryCollection, string> results = new Tuple<SubtitleEntryCollection, string>(subtitles, subtitles.ToString());
            e.Result = results;
            return;
        }
        private void WorkerApplyOffset_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            try
            {
                Tuple<SubtitleEntryCollection, string> results = args.Result as Tuple<SubtitleEntryCollection, string>;
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
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        private void WorkerTranslate_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<SubtitleEntryCollection, ITranslationProvider, string> values = e.Argument as Tuple<SubtitleEntryCollection, ITranslationProvider, string>;
            Task<SubtitleEntryCollection> task = TranslationProviderTools.ReplaceContent(values.Item1, values.Item2, values.Item3);
            task.Wait();
            e.Result = task.Result;
        }
        private void WorkerTranslate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            try
            {
                if (args.Error != null)
                    throw args.Error;
                this.Subtitles = args.Result as SubtitleEntryCollection;
                this.InvokePropertyChangedEvent(nameof(this.Busy));
            }
            catch(Exception e)
            {
                // TODO 12/9/20: handle exception
            }
            return;
        }
        #endregion
    }
}

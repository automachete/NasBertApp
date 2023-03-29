using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NasBertApp.Models;

namespace NasBertApp.ViewModels
{
    internal class MainViewModel : NotificationObject
    {
        private string _dataSetPath = string.Empty;
        public string DataSetPath
        {
            get { return this._dataSetPath; }
            set { SetProperty(ref this._dataSetPath, value); }
        }

        private string _saveFolderPath = string.Empty;
        public string SaveFolderPath
        {
            get { return this._saveFolderPath; }
            set { SetProperty(ref this._saveFolderPath, value); }
        }

        private string _modelName = string.Empty;
        public string ModelName
        {
            get { return this._modelName; }
            set { SetProperty(ref this._modelName, value); }
        }

        private bool _trainingProgressVisibled;
        public bool TrainingProgressVisibled
        {
            get { return this._trainingProgressVisibled; }
            set { SetProperty(ref this._trainingProgressVisibled, value); }
        }

        private string _modelPath = string.Empty;
        public string ModelPath
        {
            get { return this._modelPath; }
            set { SetProperty(ref this._modelPath, value); }
        }

        private string _inputText = string.Empty;
        public string InputText
        {
            get { return this._inputText; }
            set { SetProperty(ref this._inputText, value); }
        }

        private string _resultClass = string.Empty;
        public string ResultClass
        {
            get { return this._resultClass; }
            set { SetProperty(ref this._resultClass, value); }
            }

        private float _maxScore;
        public float MaxScore
        {
            get { return this._maxScore; }
            set { SetProperty(ref this._maxScore, value); }
        }

        private bool _classifyProgressVisibled;
        public bool ClassifyProgressVisibled
        {
            get { return this._classifyProgressVisibled; }
            set { SetProperty(ref this._classifyProgressVisibled, value); }
        }

        private bool _resultVisibled;
        public bool ResultVisibled
        {
            get { return this._resultVisibled; }
            set { SetProperty(ref this._resultVisibled, value); }
        }

        public MainViewModel()
        {
            this.TrainingProgressVisibled = false;
            this.ClassifyProgressVisibled = false;
            this.ResultVisibled = false;
        }

        private DelegateCommand? _choiceDataSetCommand;
        public DelegateCommand ChoiceDataSetCommand
        {
            get
            {
                return this._choiceDataSetCommand ?? (this._choiceDataSetCommand = new DelegateCommand(
                _ =>
                {
                    var FileDialog = SaveAction.GetCommonOpenFileDialog();
                    this.DataSetPath = SaveAction.GetSavePath(FileDialog, this.DataSetPath);
                }));
            }
        }

        private DelegateCommand? _choiceSaveFolderCommand;
        public DelegateCommand ChoiceSaveFolderCommand
        {
            get
            {
                return this._choiceSaveFolderCommand ?? (this._choiceSaveFolderCommand = new DelegateCommand(
                _ =>
                {
                    var FileDialog = SaveAction.GetCommonOpenFolderDialog();
                    this.SaveFolderPath = SaveAction.GetSavePath(FileDialog, this.SaveFolderPath);
                }));
            }
        }

        private DelegateCommand? _trainingCommand;
        public DelegateCommand TrainingCommand
        {
            get
            {
                return this._trainingCommand ?? (this._trainingCommand = new DelegateCommand(
                _ =>
                {
                    this.TrainingProgressVisibled = true;
                    this._textClassifier = new TextClassifier()
                    {
                        DataSetPath = this.DataSetPath,
                        SavaFolderPath = this.SaveFolderPath,
                        ModelName = this.ModelName,
                    };
                    Task trainingModel = this._textClassifier.TrainingModelAsync();
                    trainingModel.ContinueWith(_ =>
                    {
                        this.TrainingProgressVisibled = false;
                    });
                },
                _ =>
                {
                    return true;
                }));
            }
        }
        private TextClassifier? _textClassifier;

        private DelegateCommand? _choiceModelCommand;
        public DelegateCommand ChoiceModelCommand
        {
            get
            {
                return this._choiceModelCommand ?? (this._choiceModelCommand = new DelegateCommand(
                _ =>
                {
                    var FileDialog = SaveAction.GetCommonOpenFileDialog();
                    this.ModelPath = SaveAction.GetSavePath(FileDialog, this.ModelPath);
                }));
            }
        }

        private DelegateCommand? _classifyCommand;
        public DelegateCommand ClassifyCommand
        {
            get
            {
                return this._classifyCommand ?? (this._classifyCommand = new DelegateCommand(
                _ =>
                {
                    this.ClassifyProgressVisibled = true;
                    this._textClassifier = new TextClassifier()
                    {
                        ModelPath = this.ModelPath,
                        InputText = this.InputText,
                    };
                    Task classifyText = this._textClassifier.ClassifyTextAsync();
                    classifyText.ContinueWith(_ =>
                    {
                        this.ResultClass = this._textClassifier.ResultClass;
                        this.MaxScore= this._textClassifier.MaxScore;
                        this.ClassifyProgressVisibled = false;
                        this.ResultVisibled = true;
                    });
                },
                _ =>
                {
                    return true;
                }));
            }
        }
    }
}

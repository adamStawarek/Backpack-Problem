using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BackpackProblem.Viewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region fields
        private readonly string _dataSetDirectory = Directory.GetCurrentDirectory() + "/DataSets";
        private Container _container;
        private Subset _subset;
        private string _selectedDataSet;

        #endregion

        #region properties
        public ObservableCollection<string> DataSets { get; set; }
        public Container Container
        {
            get => _container;
            set
            {
                _container = value;
                RaisePropertyChanged();
            }
        }
        public Subset Subset
        {
            get => _subset;
            set
            {
                _subset = value;
                RaisePropertyChanged();
            }
        }
        private Visibility _spinnerVisibility;
        public Visibility SpinnerVisibility
        {
            get => _spinnerVisibility;
            set
            {
                _spinnerVisibility = value;
                RaisePropertyChanged();
            }
        }
        private long? _executionTime;
        public long? ExecutionTime
        {
            get => _executionTime;
            set
            {
                _executionTime = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region commands
        public RelayCommand UploadDataSetCommand { get; set; }
        public RelayCommand<object> FindBestSubsetCommand { get; set; }
        #endregion

        public MainViewModel()
        {
            Directory.CreateDirectory(_dataSetDirectory);
            UploadDataSetCommand = new RelayCommand(UploadDataSet);
            FindBestSubsetCommand = new RelayCommand<object>(FindBestSubset);
            DataSets = new ObservableCollection<string>(this.GetDataSets());
            SpinnerVisibility = Visibility.Hidden;
        }

        private IEnumerable<string> GetDataSets()
        {
            return Directory.GetFiles(_dataSetDirectory);
        }

        private void UploadDataSet()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() != true) return;

            foreach (string filename in openFileDialog.FileNames)
            {
                File.Copy(filename, Path.Combine(_dataSetDirectory, Path.GetFileName(filename)), true);
                DataSets.Add(filename);
            }
        }

        private async void FindBestSubset(object obj)
        {
            var selectedDataSet = obj as string;
            if (string.IsNullOrEmpty(selectedDataSet)) return;

            Container = null;
            Subset = null;
            ExecutionTime = null;
            SpinnerVisibility = Visibility.Visible;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var result = await Task.Run(() =>
            {
                var container = ContainerFactory.ReadFromFile(selectedDataSet);
                container.GeneratePowerSet();
                container.SortSubsets();
                var subset = container.FindBestSubset();
                return (container, subset);
            });

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            Container = result.container;
            Subset = result.subset;
            ExecutionTime = elapsedMs;

            SpinnerVisibility = Visibility.Hidden;
        }
    }
}
using SerialNumberCreator;
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SerialNumberCreator
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        //Event, um die Message-Box zur Inputvalidierung zu triggern
        public event EventHandler MissingData;
        public event EventHandler LoadingError;
        public event EventHandler SavingError;

        public MainWindowViewModel()
        {
            CreateHashCommand = new DelegateCommand(
                (o) =>
                    //Die Prüfung des Datum auf ein leeres Inputfeld funktioniert so nicht
                    !CurrentDate.ToString().Equals("") &&
                    !WorkingTitle.Equals("") &&
                    !WorkingPlace.Equals("") &&
                    !Builder.Equals("")
                ,
                (o) =>
                {
                    //Durch die canExecute Definition dürfte diese Fehlernachricht nie geworfen werden.
                    //Um dies zu provozieren muss mindestens eine Bedungung der canExecute Definition auskommentiert werden.
                    if (String.IsNullOrWhiteSpace(WorkingTitle) || String.IsNullOrWhiteSpace(WorkingPlace) || String.IsNullOrWhiteSpace(Builder))
                    {
                        MissingData?.Invoke(this, EventArgs.Empty);
                    }
                    else {
                        ListSerialHash = CreateMD5Hash();
                    }
                }
            );

            SaveHashCommand = new DelegateCommand(
                (o) =>
                    !ListSerialHash.Equals("") &&
                    //Die Prüfung des Datum auf ein leeres Inputfeld funktioniert so nicht
                    !CurrentDate.ToString().Equals("") &&
                    !WorkingTitle.Equals("") &&
                    !WorkingPlace.Equals("") &&
                    !Builder.Equals(""),
                (o) => { SaveSerialNumber(); }
            );

            LoadExistingSerialHashCommand = new DelegateCommand(
                (o) =>
                !ListSerialHash.Equals(""),
                (o) =>
                {
                    LoadSerialNumber(ListSerialHash);
                }
            );

            LoadAllExistingSerialHashCommand = new DelegateCommand(
                (o) => true,
                (o) =>
                {
                    LoadAllSerialNumbers();
                }
                
            );

            CurrentDate = DateTime.Now.Date;
            WorkingTitle = "Muster Projekt";
            WorkingPlace = "Musterstadt";
            Builder = "Max Mustermann";
            ListSerialHash = CreateMD5Hash();


        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        

        private DateTime currentDate = DateTime.Now.Date;
        private string builder = "";
        private string workingTitle = "";
        private string workingPlace = "";
        private string listSerialHash = "";
        private string infoLabel = "";
        private ObservableCollection<SerialNumberSet> existingSerialNumbers = new ObservableCollection<SerialNumberSet>();

        public string InfoLabel
        {
            get => infoLabel;
            set
            {
                if (!infoLabel.Equals(value))
                {
                    infoLabel = value;
                    RaisePropertyChanged();                   
                }
            }
        }
        public ObservableCollection<SerialNumberSet> ExistingSerialNumbers
        {
            get => existingSerialNumbers;
            set
            {
                if (existingSerialNumbers != value)
                {
                    existingSerialNumbers = value;
                    RaisePropertyChanged();
                }
            }
        }
        public DateTime CurrentDate
        {
            get => currentDate;
            set
            {
                if (currentDate != value)
                { 
                    currentDate = value;
                    RaisePropertyChanged();
                    CreateHashCommand.RaiseCanExecuteChanged();
                    SaveHashCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string Builder
        {
            get => builder;
            set
            {
                if (!builder.Equals(value))
                {
                    builder = value;
                    RaisePropertyChanged();
                    CreateHashCommand.RaiseCanExecuteChanged();
                    SaveHashCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string WorkingTitle
        {
            get => workingTitle;
            set
            {
                if (!workingTitle.Equals(value))
                {
                    workingTitle = value;
                    RaisePropertyChanged();
                    CreateHashCommand.RaiseCanExecuteChanged();
                    SaveHashCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string WorkingPlace
        {
            get => workingPlace;
            set
            {
                if (!workingPlace.Equals(value))
                {
                    workingPlace = value;
                    RaisePropertyChanged();
                    CreateHashCommand.RaiseCanExecuteChanged();
                    SaveHashCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string ListSerialHash
        {
            get => listSerialHash;
            set
            {
                if (!listSerialHash.Equals(value))
                {
                    listSerialHash = value;
                    RaisePropertyChanged();
                    SaveHashCommand.RaiseCanExecuteChanged();
                    LoadExistingSerialHashCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand CreateHashCommand { get; set; }

        public DelegateCommand SaveHashCommand { get; set; }

        public DelegateCommand LoadExistingSerialHashCommand { get; set; }

        public DelegateCommand LoadAllExistingSerialHashCommand { get; set; }

        private string CreateMD5Hash() {


            string inputVlaue = CurrentDate.ToString() + Builder + WorkingTitle + WorkingPlace;

            byte[] hash;
            StringBuilder sb = new StringBuilder();

            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(inputVlaue));
            }

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString().ToUpper();


        }

        private void SaveSerialNumber()
        {
            //Prüfung, ob der MD5 mit den eingaben berechnet werden kann.
            string shouldMD5 = CreateMD5Hash();
            string isMD5 = listSerialHash;

            if (!isMD5.Equals(shouldMD5))
            {
                ListSerialHash = shouldMD5;
            }
                        
            SerialNumberSet serielNumberSet = new SerialNumberSet(this.CurrentDate, this.Builder, this.WorkingTitle, this.WorkingPlace, this.ListSerialHash);
            string jsonText = serielNumberSet.toJson();
            string jsonHash = serielNumberSet.SerialHash;
            string storagePath = "./Serials/";

            try
            {
                if (!Directory.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                }

                string jsonPath = storagePath + "serial_" + jsonHash + ".json";
                File.WriteAllText(jsonPath, jsonText);
                InfoLabel = "Created serial saved.";
            }
            catch (Exception ex)
            {
                InfoLabel = "Saving created serial fails.";
                SavingError.Invoke(this, EventArgs.Empty);
            }
 
        }

        private void LoadSerialNumber(string jsonHash)
        {
            string filePath = "./Serials/";
            string jsonPath = filePath + "serial_" + jsonHash + ".json";

            if (Directory.Exists(filePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(jsonPath);
                    SerialNumberSet set;
                    set = JsonSerializer.Deserialize<SerialNumberSet>(jsonString)!;
                    existingSerialNumbers.Clear();
                    existingSerialNumbers.Add(set);
                    InfoLabel = "Serial found. Loading successfull.";
                }
                catch (Exception ex)
                {
                    existingSerialNumbers.Clear();
                    InfoLabel = "Serial not found.";
                    LoadingError.Invoke(this, EventArgs.Empty);
                }
            }
            else {
                existingSerialNumbers.Clear();
                InfoLabel = "Could not find serial directory.";
            }

        }

        private void LoadAllSerialNumbers()
        {
            string filePath = "./Serials/";
            string[] files;

            if (Directory.Exists(filePath))
            {
                files = Directory.GetFileSystemEntries(filePath, "serial_*");
                existingSerialNumbers.Clear();

                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        try
                        {
                            string jsonString = File.ReadAllText(file);
                            SerialNumberSet set;
                            set = JsonSerializer.Deserialize<SerialNumberSet>(jsonString)!;
                            existingSerialNumbers.Add(set);
                            InfoLabel = files.Length + " Serials loaded.";
                        }
                        catch (Exception ex)
                        {
                            existingSerialNumbers.Clear();
                            InfoLabel = "Loading serials failed.";
                            LoadingError.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
                else {
                    existingSerialNumbers.Clear();
                    InfoLabel = "No serials found.";
                }
                
            }
            else
            {
                existingSerialNumbers.Clear();
                InfoLabel = "Could not find serial directory.";
            }

        }
    }
 
}

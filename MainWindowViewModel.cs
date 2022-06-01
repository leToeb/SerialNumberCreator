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
                (o) => {
                    ListSerialHash = CreateMD5Hash();
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
        private ObservableCollection<SerialNumberSet> existingSerialNumbers = new ObservableCollection<SerialNumberSet>();

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
            string jsonTitel = serielNumberSet.SerialHash;
            //Hier sollte ein Ordner erstellt werden, in den die Serials gespeichert werden.
            string storagePath = "./Serials/";

            if (!Directory.Exists(storagePath))
            { 
                Directory.CreateDirectory(storagePath);
            }

            string jsonPath = storagePath + jsonTitel + ".json";
            File.WriteAllText(jsonPath, jsonText);
        }

        private void LoadSerialNumber(string jsonTitel)
        {
            string jsonPath = "./Serials/" + jsonTitel + ".json";
            try
            {
                string jsonString = File.ReadAllText(jsonPath);
                SerialNumberSet set;
                set = JsonSerializer.Deserialize<SerialNumberSet>(jsonString)!;
                existingSerialNumbers.Clear();
                existingSerialNumbers.Add(set);
            }
            catch (Exception ex)
            {
                existingSerialNumbers.Clear();
                //TODO Ausnahmebehandlung

            }

        }
    }
 
}

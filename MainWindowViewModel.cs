using SerialNumberCreator;
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

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
                (o) => !ListSerialHash.Equals(""),
                (o) => { SaveSerialNumber(); }
            );

            CurrentDate = DateTime.Now;
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
                //Der Create Button soll ausgebaut werdne. Der Hash muss sich bei Propertyänderung aktualisieren
                //ListSerialHash = CreateMD5Hash();
            }

        }
        

        DateTime currentDate = DateTime.Now;
        string builder = "";
        string workingTitle = "";
        string workingPlace = "";
        string listSerialHash = "";

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
                    listSerialHash = value.ToUpper();
                    RaisePropertyChanged();
                    SaveHashCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand CreateHashCommand { get; set; }

        public DelegateCommand SaveHashCommand { get; set; }

        private string CreateMD5Hash() {


            string inputVlaue = CurrentDate.ToShortDateString() + Builder + WorkingTitle + WorkingPlace;

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

            return sb.ToString();


        }

        private void SaveSerialNumber()
        {
            SerialNumberSet serielNumberSet = new SerialNumberSet(this.CurrentDate, this.Builder, this.WorkingTitle, this.WorkingPlace, this.ListSerialHash);
            string jsonText = serielNumberSet.toJson();
            string jsonTitel = serielNumberSet.SerialHash;
            //Hier sollte ein Ordner erstellt werden, in den die Serials gespeichert werden.
            string jsonPath = "./" + jsonTitel + ".json";
            File.WriteAllText(jsonPath, jsonText);
        }
    }
 
}

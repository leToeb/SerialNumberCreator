using System;
using System.Text.Json;

namespace SerialNumberCreator
{
    internal class SerialNumberSet
    {
        public DateTime Date { get; set; }
        public string Builder { get; set; }
        public string WorkingTitle { get; set; }
        public string WorkingPlace { get; set; }
        public string SerialHash { get; set; }

        public SerialNumberSet()
        { }
        public SerialNumberSet(DateTime date, string builder, string workingTitle, string workingPlace, string serialHash)
        {
            Date = date;
            Builder = builder;
            WorkingTitle = workingTitle;
            WorkingPlace = workingPlace;
            SerialHash = serialHash;
        }

        public string toJson()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}

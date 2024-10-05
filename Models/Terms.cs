using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using C971.Models;
using System.Collections.ObjectModel;

namespace C971.Models
{
    public class Terms : INotifyPropertyChanged
    {
        public string termTitle;
        public DateTime start;
        public DateTime end;

        [PrimaryKey, AutoIncrement]
        public int termId { get; set; }

        public string TermTitle
        {
            get => termTitle;
            set
            {
                termTitle = value;
                OnPropertyChanged(nameof(termTitle));
            }
        }

        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged(nameof(start));
            }
        }

        public DateTime End
        {
            get => end;
            set
            {
                end = value;
                OnPropertyChanged(nameof(end));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

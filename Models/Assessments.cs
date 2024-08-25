using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Models
{
    public  class Assessments
    {
        public string assessmentName;
        public int assessmentId;
        public DateTime start;
        public DateTime end;

        [PrimaryKey, AutoIncrement]

        public int AssessmentId
        {
            get => assessmentId;
            set
            {
                assessmentId = value;
                OnPropertyChanged(nameof(AssessmentId));
            }
        }

        public string AssessmentName
        {
            get => assessmentName;
            set
            {
                assessmentName = value;
                OnPropertyChanged(nameof(AssessmentName));
            }
        }

        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        public DateTime End
        {
            get => end;
            set
            {
                end = value;
                OnPropertyChanged(nameof(End));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

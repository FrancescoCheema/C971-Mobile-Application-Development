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
        [PrimaryKey, AutoIncrement]
        public int assessmentId { get; set; }

        public string performanceAssessmentName;
        public string objectiveAssessmentName;
        public DateTime start;
        public DateTime end;
        public DateTime obstart;
        public DateTime obend;
        public bool _notificationsEnabled { get; set; }

        public DateTime ObEnd  
        {
            get => obend;
            set
            {
                obend = value;
                OnPropertyChanged(nameof(ObEnd));
            }
        }

        public DateTime ObStart
        {
            get => obstart;
            set
            {
                obstart = value;
                OnPropertyChanged(nameof(ObStart));
            }
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (_notificationsEnabled != value)
                {
                    _notificationsEnabled = value;
                    OnPropertyChanged(nameof(NotificationsEnabled));
                }
            }
        }


        public int CourseId
        {
            get => courseId;
            set
            {
                courseId = value;
                OnPropertyChanged(nameof(CourseId));
            }
        }

        public string ObjectiveAssessmentName
        {
            get => objectiveAssessmentName;
            set
            {
                objectiveAssessmentName = value;
                OnPropertyChanged(nameof(ObjectiveAssessmentName));
            }
        }

        public string PerformanceAssessmentName
        {
            get => performanceAssessmentName;
            set
            {
                performanceAssessmentName = value;
                OnPropertyChanged(nameof(PerformanceAssessmentName));
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

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
       
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int courseId; 
    }
}

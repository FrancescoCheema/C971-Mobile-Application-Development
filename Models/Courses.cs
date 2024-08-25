using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SQLite;
using C971.Models;

namespace C971.Models
{
    public class Courses : INotifyPropertyChanged
    {
        public int courseId;
        public string courseTitle;
        public string courseNotes;
        public DateTime startCourse;
        public DateTime endCourse;
        public string courseStatus;
        public string instructorName;
        public string instructorPhone;
        public string instructorEmail;
        public string selectedStatus;
        public int termId;

        [PrimaryKey, AutoIncrement]
        public int CourseId
        {
            get => courseId;
            set
            {
                courseId = value;
                OnPropertyChanged(nameof(CourseId));
            }
        }

        public int TermId
        {
            get => termId;
            set
            {
                termId = value;
                OnPropertyChanged(nameof(TermId));
            }
        }

        public string CourseTitle
        {
            get => courseTitle;
            set
            {
                courseTitle = value;
                OnPropertyChanged(nameof(CourseTitle));
            }
        }

        public string CourseNotes
        {
            get => courseNotes;
            set
            {
                courseNotes = value;
                OnPropertyChanged(nameof(CourseNotes));
            }
        }

        public DateTime StartCourse
        {
            get => startCourse;
            set
            {
                startCourse = value;
                OnPropertyChanged(nameof(StartCourse));
            }
        }

        public DateTime EndCourse
        {
            get => endCourse;
            set
            {
                endCourse = value;
                OnPropertyChanged(nameof(EndCourse));
            }
        }

        public string CourseStatus
        {
            get => courseStatus;
            set
            {
                courseStatus = value;
                OnPropertyChanged(nameof(CourseStatus));
            }
        }

        public string InstructorName
        {
            get => instructorName;
            set
            {
                instructorName = value;
                OnPropertyChanged(nameof(InstructorName));
            }
        }

        public string InstructorPhone
        {
            get => instructorPhone;
            set
            {
                instructorPhone = value;
                OnPropertyChanged(nameof(InstructorPhone));
            }
        }

        public string InstructorEmail
        {
            get => instructorEmail;
            set
            {
                instructorEmail = value;
                OnPropertyChanged(nameof(InstructorEmail));
            }
        }

        public string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                if (selectedStatus != value)
                {
                    selectedStatus = value;
                    CourseStatus = value;
                    OnPropertyChanged(nameof(SelectedStatus));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
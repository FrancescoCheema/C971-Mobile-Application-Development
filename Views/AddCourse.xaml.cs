using C971.Models;
using C971.Services;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace C971.Views;

public partial class AddCourse : ContentPage
{
    public Courses Course { get; set; } = new Courses();
    public Courses selectedCourse { get; set; }

    public Courses courses;
    public Courses newCourse { get; set; }

    private string _courseId;
    private DatabaseService _databaseService;
    public ObservableCollection<Courses> _coursesList;

    private DateTime _startCourseDate;
    public DateTime StartCourseDate
    {
        get => _startCourseDate;
        set
        {
            if (_startCourseDate != value)
            {
                _startCourseDate = value;
                EndCourseDate = _startCourseDate.AddMonths(1);
                OnPropertyChanged(nameof(StartCourseDate));
            }
        }
    }

    private DateTime _endCourseDate;
    public DateTime EndCourseDate
    {
        get => _endCourseDate;
        set
        {
            if (_endCourseDate != value)
            {
                _endCourseDate = value;
                OnPropertyChanged(nameof(EndCourseDate));
            }
        }
    }

    private List<Courses> allCourses;
    public ObservableCollection<Courses> FilteredCourses { get; set; }

    public AddCourse(DatabaseService databaseService, int courseId, ObservableCollection<Courses> coursesList)
    {
        InitializeComponent();
        FilteredCourses = new ObservableCollection<Courses>();
        _databaseService = databaseService;
        _courseId = courseId.ToString();
        _coursesList = coursesList;
        StartCourseDate = DateTime.Now;
        EndCourseDate = StartCourseDate.AddMonths(1);
        courses = new Courses();
        newCourse = new Courses();
        selectedCourse = new Courses();
        BindingContext = this;


        courseTitleLabel.Unfocused += CourseTitleLabel_Unfocused;
        instructorField.Unfocused += InstructorField_Unfocused;
        phoneField.Unfocused += PhoneField_Unfocused;
        emailField.Unfocused += EmailField_Unfocused;
        courseNotesEntry.Unfocused += CourseNotesEntry_Unfocused;

        if (selectedCourse != null)
        {
            notificationsSwitch.IsToggled = selectedCourse.NotificationsEnabled;
        }
        else
        {
            selectedCourse = new Courses();
            notificationsSwitch.IsToggled = false;
        }
        notificationsSwitch.IsEnabled = Course.NotificationsEnabled;
        notificationsSwitch.IsEnabled = true;
    }


    private void CourseTitleLabel_Unfocused(object sender, FocusEventArgs e)
    {
        CheckForm();
    }

    private void InstructorField_Unfocused(object sender, FocusEventArgs e)
    {
        CheckForm();
    }

    private void CourseNotesEntry_Unfocused(object sender, FocusEventArgs e)
    {
        CheckForm();
    }


    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        CheckForm();
    }

    private void CourseNotesEntry_TextChanged(object sender, EventArgs e)
    {
        CheckForm();
    }

    bool isNotificationsToggled = true;

    bool isPhoneChanged = false;
    private void PhoneField_Unfocused(object sender, FocusEventArgs e)
    {
        isPhoneChanged = true;
        CheckForm();
    }

    private void EmailField_Unfocused(object sender, FocusEventArgs e)
    {
        CheckForm();
    }


    private async void CheckForm()
    {
        Regex regex = new Regex(@"([\-]?\d[\-]?){10}");
        Regex regexEmail = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");

        bool isValidPhone = !string.IsNullOrEmpty(phoneField.Text) && regex.IsMatch(phoneField.Text) && phoneField.Text.Length == 10;
        bool isValidEmail = !string.IsNullOrEmpty(emailField.Text) && regexEmail.IsMatch(emailField.Text);

        if (string.IsNullOrEmpty(courseTitleLabel.Text))
        {
            await DisplayAlert("Missing course name", "Please Enter a Name", "Ok");
            saveCourse.IsEnabled = false;
        }

        else if (string.IsNullOrWhiteSpace(courseNotesEntry.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            return;
        }

        else if (string.IsNullOrEmpty(instructorField.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            saveCourse.IsEnabled = false;
        }
        else if (isPhoneChanged && !isValidPhone)
        {
            await DisplayAlert("Invalid phone number", "Please Enter a valid phone number", "Ok");
            saveCourse.IsEnabled = false;
        }
        else if (!isValidEmail)
        {
            await DisplayAlert("Invalid email address", "Please Enter a valid email address", "Ok");
            saveCourse.IsEnabled = false;
        }
        else
        {
            saveCourse.IsEnabled = true;
        }
    }

    async void Button_Clicked(object sender, EventArgs e)
    {
        saveCourse.IsEnabled = true;

        isPhoneChanged = false;


        selectedCourse.CourseTitle = courseTitleLabel.Text;
        selectedCourse.StartCourse = CoursePicker.Date;
        selectedCourse.EndCourse = CourseEndPicker.Date;
        selectedCourse.CourseNotes = courseNotesEntry?.Text;
        selectedCourse.CourseStatus = picker.SelectedItem?.ToString();
        selectedCourse.InstructorName = instructorField.Text;
        selectedCourse.InstructorPhone = phoneField?.Text;
        selectedCourse.InstructorEmail = emailField.Text;
        selectedCourse.NotificationsEnabled = notificationsSwitch.IsToggled;

        try
        {
            await _databaseService.AddCourseAsync(selectedCourse);

            MessagingCenter.Send(this, "CourseAdded", selectedCourse);
            await DisplayAlert("Success", "Course saved successfully!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save course: {ex.Message}", "OK");
        }
    }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            if (selectedCourse != null)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Title = "Share Course Notes",
                    Text = $"Course Title: {selectedCourse.CourseTitle}\n" +
                           $"Course Notes: {selectedCourse.CourseNotes}\n" +
                           $"Start Date: {selectedCourse.StartCourse}\n" +
                           $"End Date: {selectedCourse.EndCourse}\n" +
                           $"Instructor: {selectedCourse.InstructorName}\n" +
                           $"Instructor Email: {selectedCourse.InstructorEmail}\n" +
                           $"Instructor Phone: {selectedCourse.InstructorPhone}"
                });
            }
        }

        async Task ViewCourseInDatabase()
        {
            List<Courses> course = await _databaseService.GetCourses();
            foreach (var courses in course)
            {
                _coursesList.Add(courses);
            }
        }

        private void ScheduleNotifications()
        {
            var startNotification = new NotificationRequest
            {
                NotificationId = 1000,
                Title = "Course Start Reminder",
                Description = $"Your Course '{selectedCourse.CourseTitle}' is starting today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = selectedCourse.StartCourse
                }
            };
            LocalNotificationCenter.Current.Show(startNotification);

            var endNotification = new NotificationRequest
            {
                NotificationId = 1001,
                Title = "Course End Reminder",
                Description = $"Your Course '{selectedCourse.CourseTitle}' is ending today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = selectedCourse.EndCourse
                }
            };
            LocalNotificationCenter.Current.Show(startNotification);
        }

        private void Notifications_Toggled(object sender, ToggledEventArgs e)
        {
            Course.NotificationsEnabled = e.Value;

            if (e.Value)
            {
                DisplayAlert("Alert", "The notifications for the start and end course have been turned on.", "OK");
                ScheduleNotifications();
            }
            else
            {
                DisplayAlert("Alert", "The notifications for the start and end course have been turned off.", "OK");
                CancelNotifications();
            }
        }

        private void CancelNotifications()
        {
            LocalNotificationCenter.Current.Cancel(1000);
            LocalNotificationCenter.Current.Cancel(1001);
        }

        private async void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value && selectedCourse != null)
            {
                await Navigation.PushAsync(new ObjectiveAssessment(selectedCourse));
            }
        }

        private async void RadioButton_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value && selectedCourse != null)
            {
                await Navigation.PushAsync(new PerformanceAssessment(selectedCourse));
            }
        }
    }

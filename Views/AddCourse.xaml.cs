using C971.Models;
using C971.Services;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace C971.Views;

public partial class AddCourse : ContentPage
{
    public Courses Course { get; set; } = new Courses();
    public Courses selectedCourse { get; set; }

    private string _courseId;
    private DatabaseService _databaseService;
    private ObservableCollection<Courses> _coursesList;

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

    public AddCourse(DatabaseService databaseService, int courseId, ObservableCollection<Courses> coursesList)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _courseId = courseId.ToString();
        _coursesList = coursesList;
        StartCourseDate = DateTime.Now;
        EndCourseDate = StartCourseDate.AddMonths(1);
        BindingContext = this;


        courseTitleLabel.TextChanged += CourseTitleLabel_TextChanged;
        instructorField.TextChanged += InstructorField_TextChanged;
        phoneField.TextChanged += PhoneField_TextChanged;
        emailField.TextChanged += EmailField_TextChanged;
    }

    private void CourseTitleLabel_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private void InstructorField_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private void PhoneField_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private void EmailField_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        CheckForm();
    }


    private async void CheckForm()
    {
        Regex regex = new Regex(@"([\-]?\d[\-]?){10}");
        Match match = regex.Match(phoneField.Text);

        Regex regexEmail = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
        Match emailMatch = regexEmail.Match(emailField.Text);


        if (string.IsNullOrWhiteSpace(courseTitleLabel.Text))
        {
            await DisplayAlert("Missing course name", "Please Enter a Name", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(instructorField.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }


        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(emailField.Text) || emailMatch.Success == false)
        {
            await DisplayAlert("Missing email address", "Please Enter a valid email address", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }
        else
        {
            saveCourse.IsEnabled = true;
        }
    }


    async void Button_Clicked(object sender, EventArgs e)
    {
        Regex regex = new Regex(@"([\-]?\d[\-]?){10}");
        Match match = regex.Match(phoneField.Text);

        Regex regexEmail = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
        Match emailMatch = regexEmail.Match(emailField.Text);


        if (string.IsNullOrWhiteSpace(courseTitleLabel.Text))
        {
            await DisplayAlert("Missing course name", "Please Enter a Name", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(instructorField.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }


        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(emailField.Text) || emailMatch.Success == false)
        {
            await DisplayAlert("Missing email address", "Please Enter a valid email address", "Ok");
            saveCourse.IsEnabled = false;
            return;
        }
        else
        {

            await _databaseService.Init();

            Courses newCourse = new Courses
            {
                CourseTitle = courseTitleLabel.Text,
                StartCourse = CoursePicker.Date,
                EndCourse = CourseEndPicker.Date,
                CourseStatus = picker.SelectedItem?.ToString(),
                InstructorName = instructorField.Text,
                InstructorPhone = phoneField?.Text,
                InstructorEmail = emailField.Text
            };

            try
            {
                await _databaseService.AddCourseAsync(newCourse);
                _coursesList.Add(newCourse);
                ViewCourseInDatabase();
                await DisplayAlert("Success", "Course saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save the course: {ex.Message}", "OK");
            }
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
            Description = $"Your Course '{Course.CourseTitle}' is starting today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = Course.StartCourse
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);

        var endNotification = new NotificationRequest
        {
            NotificationId = 1001,
            Title = "Course End Reminder",
            Description = $"Your Course '{Course.CourseTitle}' is ending today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = Course.EndCourse
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);
    }

    private void Notifications_Toggled(object sender, ToggledEventArgs e)
    {
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
            await Navigation.PushAsync(new PeformanceAssessment(selectedCourse));
        }
    }
}

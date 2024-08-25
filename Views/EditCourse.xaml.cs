using C971.Models;
using C971.Services;
using Plugin.LocalNotification;
using System.Text.RegularExpressions;

namespace C971.Views;

public partial class EditCourse : ContentPage
{
    public Courses selectedCourse { get; set; }

    public DatabaseService databaseService;

    public Courses courses;

    List<Courses> coursesList;

    int termId;
    public EditCourse(Courses course, DatabaseService dbService)
    {
        InitializeComponent();
        courses = course;
        selectedCourse = course;
        databaseService = dbService;
        BindingContext = selectedCourse;


        var statusList = new List<string>
        {
            "In Progress",
            "Dropped",
            "Completed",
            "Planned to take"
        };

        Picker picker = new Picker { Title = "Select a status" };
        picker.ItemsSource = statusList;

        
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
            saveButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(instructorField.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveButton.IsEnabled = false;
            return;
        }


        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(emailField.Text) || emailMatch.Success == false)
        {
            await DisplayAlert("Missing email address", "Please Enter a valid email address", "Ok");
            saveButton.IsEnabled = false;
            return;
        }
        else
        {
            saveButton.IsEnabled = true;
        }
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        Regex regex = new Regex(@"([\-]?\d[\-]?){10}");
        Match match = regex.Match(phoneField.Text);

        Regex regexEmail = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
        Match emailMatch = regexEmail.Match(emailField.Text);


        if (string.IsNullOrWhiteSpace(courseTitleLabel.Text))
        {
            await DisplayAlert("Missing course name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(instructorField.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveButton.IsEnabled = false;
            return;
        }


        if (string.IsNullOrWhiteSpace(phoneField.Text) || match.Success == false || phoneField.Text.Length > 10)
        {
            await DisplayAlert("Missing phone number", "Please Enter a valid phone number", "Ok");
            saveButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(emailField.Text) || emailMatch.Success == false)
        {
            await DisplayAlert("Missing email address", "Please Enter a valid email address", "Ok");
            saveButton.IsEnabled = false;
            return;
        }
        else
        {
            saveButton.IsEnabled = true;

            courses.CourseTitle = courseTitleLabel.Text;
            courses.startCourse = CoursePicker.Date;
            courses.endCourse = CourseEndPicker.Date;
            courses.CourseStatus = picker.ItemsSource?.ToString();
            courses.InstructorName = instructorField.Text;
            courses.InstructorPhone = phoneField?.Text;
            courses.InstructorEmail = emailField.Text;

            await databaseService.AddCourseAsync(selectedCourse);
            await DisplayAlert("Success", "Course saved successfully!", "OK");
        }
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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(page: new ViewTerm());
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
}
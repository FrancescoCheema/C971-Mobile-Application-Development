using C971.Models;
using C971.Services;
using Plugin.LocalNotification;
using System.Text.RegularExpressions;

namespace C971.Views;

public partial class EditCourse : ContentPage
{
    public Courses Course { get; set; } = new Courses();
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

        if (selectedCourse != null)
        {
            notificationsSwitch.IsToggled = selectedCourse.NotificationsEnabled;
        }
        else
        {
            selectedCourse = new Courses();
            notificationsSwitch.IsToggled = false;
        }


        var statusList = new List<string>
        {
            "In Progress",
            "Dropped",
            "Completed",
            "Planned to take"
        };

        picker.ItemsSource = statusList;

        BindingContext = selectedCourse;

        if (!string.IsNullOrEmpty(selectedCourse.CourseStatus))
        {
            picker.SelectedItem = selectedCourse.CourseStatus;
        }

        courseTitleLabel.Unfocused += CourseTitleLabel_Unfocused;
        instructorField.Unfocused += InstructorField_Unfocused;
        phoneField.Unfocused += PhoneField_Unfocused;
        emailField.Unfocused += EmailField_Unfocused;
        courseNotesEntry.Unfocused += CourseNotesEntry_Unfocused;
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

    private void Picker_SelectedIndexChanged(object sender, FocusEventArgs e)
    {
        CheckForm();
    }

    private void CourseNotesEntry_Unfocused(object sender, FocusEventArgs e)
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
            saveButton.IsEnabled = false;
        }

        else if (string.IsNullOrWhiteSpace(courseNotesEntry.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            return;
        }

        else if (string.IsNullOrEmpty(instructorField.Text))
        {
            await DisplayAlert("Missing Instructor name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
        }
        else if (isPhoneChanged && !isValidPhone)
        {
            await DisplayAlert("Invalid phone number", "Please Enter a valid phone number", "Ok");
            saveButton.IsEnabled = false;
        }
        else if (!isValidEmail)
        {
            await DisplayAlert("Invalid email address", "Please Enter a valid email address", "Ok");
            saveButton.IsEnabled = false;
        }
        else
        {
            saveButton.IsEnabled = true;
        }
    }


private async void Button_Clicked_1(object sender, EventArgs e)
{

    saveButton.IsEnabled = true;

    isPhoneChanged = false;

    courses.CourseTitle = courseTitleLabel.Text;
    courses.startCourse = CoursePicker.Date;
    courses.endCourse = CourseEndPicker.Date;
    courses.courseNotes = courseNotesEntry?.ToString();
    courses.CourseStatus = picker.SelectedItem?.ToString();
    courses.InstructorName = instructorField.Text;
    courses.InstructorPhone = phoneField?.Text;
    courses.InstructorEmail = emailField.Text;
    courses.NotificationsEnabled = notificationsSwitch.IsToggled;

    await databaseService.AddCourseAsync(selectedCourse);
    MessagingCenter.Send(this, "CourseAdded", selectedCourse);
    await DisplayAlert("Success", "Course saved successfully!", "OK");
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
}


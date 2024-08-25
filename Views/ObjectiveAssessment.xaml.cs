using C971.Models;
using C971.Services;
using Plugin.LocalNotification;

namespace C971.Views;

public partial class ObjectiveAssessment : ContentPage
{
    public Courses selectedCourse { get; set; }

    public DatabaseService databaseService;
    public ObjectiveAssessment(Courses course)
	{
		InitializeComponent();
        selectedCourse = course;
        BindingContext = selectedCourse;
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
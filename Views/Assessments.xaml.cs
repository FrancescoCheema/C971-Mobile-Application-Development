using C971.Models;
using C971.Services;
using Microsoft.Maui.Handlers;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C971.Views;

public partial class Assessments : ContentPage
{
    public Courses selectedCourse { get; set; }
    public DatabaseService databaseService;
    public ObservableCollection<Courses> CoursesList { get; set; }

    public Assessments(Courses course)
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        CoursesList = new ObservableCollection<Courses>();
        selectedCourse = course;
        BindingContext = selectedCourse;
        LoadCourses();
    }

    private async void LoadCourses()
    {
        await databaseService.Init();
        var courses = await databaseService.GetCourses();
        foreach (var course in courses)
        {
            CoursesList.Add(course);
        }
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

    private void ScheduleNotificationsOff()
    {
        var startNotification = new NotificationRequest
        {
            NotificationId = 1003,
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
            NotificationId = 1004,
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
            CancelNotifications2();
        }
    }

    private void Notifications2_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            DisplayAlert("Alert", "The notifications for the start and end course have been turned on.", "OK");
            ScheduleNotificationsOff();
        }
        else
        {
            DisplayAlert("Alert", "The notifications for the start and end course have been turned off.", "OK");
            CancelNotifications2();
        }
    }

    private void CancelNotifications()
    {
        LocalNotificationCenter.Current.Cancel(1000);
        LocalNotificationCenter.Current.Cancel(1001);
    }

    private void CancelNotifications2()
    {
        LocalNotificationCenter.Current.Cancel(1003);
        LocalNotificationCenter.Current.Cancel(1004);
    }
}
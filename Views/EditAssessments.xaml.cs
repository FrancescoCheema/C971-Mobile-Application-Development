using C971.Models;
using C971.Services;
using Plugin.LocalNotification;
using System.Text.RegularExpressions;

namespace C971.Views;

public partial class EditAssessments : ContentPage
{
    public Courses selectedCourse { get; set; }
    public Assessments Assessment { get; set; }

    public static int selectedCourseId;

    public DatabaseService databaseService;
    public Courses Course { get; set; }

    Assessments lastSelection;

    List<Assessments> assessmentList;
    public EditAssessments(Courses course, Assessments assessment)
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        Assessment = assessment;
        Course = course;
        BindingContext = Assessment;

        performanceAssessmentLabel.TextChanged += performanceAssessmentLabel_TextChanged;

        notificationsSwitch.IsToggled = Assessment.NotificationsEnabled;
    }

    private void performanceAssessmentLabel_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private async void CheckForm()
    {
        if (string.IsNullOrEmpty(performanceAssessmentLabel.Text))
        {
            await DisplayAlert("Missing assessment name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
        }
        else
        {
            saveButton.IsEnabled = true;
        }
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(performanceAssessmentLabel.Text))
        {
            await DisplayAlert("Missing assessment name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
        }
        else
        {
            saveButton.IsEnabled = true;

            Assessment.performanceAssessmentName = performanceAssessmentLabel.Text;
            Assessment.start = AssessmentsPicker.Date;
            Assessment.end = AssessmentsEndPicker.Date;
            Assessment.NotificationsEnabled = notificationsSwitch.IsToggled;

            await databaseService.UpdateAssessmentAsync(Assessment);
            MessagingCenter.Send(this, "PFAssessmentAdded", Assessment);
            await DisplayAlert("Success", "Assessment saved successfully!", "OK");
            await Navigation.PopAsync();
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
            Description = $"Your assessment '{Assessment.objectiveAssessmentName}' is starting today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = Assessment.Start
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);

        var endNotification = new NotificationRequest
        {
            NotificationId = 1001,
            Title = "Course End Reminder",
            Description = $"Your assessment '{Assessment.objectiveAssessmentName}' is ending today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = Assessment.end
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);
    }

    private void Notifications_Toggled(object sender, ToggledEventArgs e)
    {
       Assessment.NotificationsEnabled = e.Value;

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
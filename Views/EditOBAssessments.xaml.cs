using C971.Models;
using C971.Services;
using Plugin.LocalNotification;

namespace C971.Views;

public partial class EditOBAssessments : ContentPage
{
    public Assessments assessmentId { get; set; }
    public Courses selectedCourse { get; set; }
    public Assessments Assessment { get; set; }

    public static int selectedCourseId;

    public DatabaseService databaseService;
    public Courses Course { get; set; }

    Assessments lastSelection;

    List<Assessments> assessmentList;
    public EditOBAssessments(Courses course, Assessments assessment)
	{
		InitializeComponent();
        databaseService = new DatabaseService();
        selectedCourse = course;
        Assessment = assessment;
        Course = course;
        BindingContext = Assessment;

        objectiveAssessmentLabel.TextChanged += objectiveAssessmentLabel_TextChanged;

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

    private void objectiveAssessmentLabel_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private async void CheckForm()
    {
        if (string.IsNullOrEmpty(objectiveAssessmentLabel.Text))
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
        if (string.IsNullOrEmpty(objectiveAssessmentLabel.Text))
        {
            await DisplayAlert("Missing assessment name", "Please Enter a Name", "Ok");
            saveButton.IsEnabled = false;
        }
        else
        {
            saveButton.IsEnabled = true;

            Assessment.objectiveAssessmentName = objectiveAssessmentLabel.Text;
            Assessment.obstart = OBAssessmentsPicker.Date;
            Assessment.obend = OBAssessmentsEndPicker.Date;
            Assessment.NotificationsEnabled = notificationsSwitch.IsToggled;

            await databaseService.UpdateAssessmentAsync(Assessment);
            MessagingCenter.Send(this, "AssessmentAdded", Assessment);
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
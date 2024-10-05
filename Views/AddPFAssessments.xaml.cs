using C971.Models;
using C971.Services;
using Plugin.LocalNotification;


namespace C971.Views;
public partial class AddPFAssessments : ContentPage
{
    public int assessmentId { get; set; }
    public Courses selectedCourse { get; set; }
    public Assessments Assessment { get; set; }

    public static int selectedCourseId;

    public DatabaseService databaseService;
    public Courses Course { get; set; }

    Assessments lastSelection;

    List<Assessments> assessmentList;
    public AddPFAssessments(Courses course, Assessments assessment)
    {
        InitializeComponent();
        assessmentId = assessment?.assessmentId ?? 0;
        selectedCourse = course ?? new Courses();
        Assessment = assessment ?? new Assessments();
        databaseService = new DatabaseService();

        BindingContext = Assessment;

        if (selectedCourse != null)
        {
            notificationsSwitch.IsToggled = selectedCourse.NotificationsEnabled;
        }
        else
        {
            selectedCourse = new Courses();
            notificationsSwitch.IsToggled = false;
        }
        notificationsSwitch.IsEnabled = selectedCourse.NotificationsEnabled;
        notificationsSwitch.IsEnabled = true;

        PerformanceAssessmentLabel.TextChanged += PerformanceAssessmentLabel_TextChanged;

        PFAssessmentsPicker.Date = DateTime.Now;
        PFAssessmentsEndPicker.Date = DateTime.Now;
    }

    private void PerformanceAssessmentLabel_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private async void CheckForm()
    {
        if (string.IsNullOrEmpty(PerformanceAssessmentLabel.Text))
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
        if (string.IsNullOrEmpty(PerformanceAssessmentLabel.Text))
        {
            await DisplayAlert("Missing assessment name", "Please enter a name.", "OK");
            return;
        }

        try
        {
            if (Assessment.CourseId >= 0)
            {
                Assessment.CourseId = selectedCourse.CourseId;
                Assessment.PerformanceAssessmentName = PerformanceAssessmentLabel.Text;
                Assessment.obstart = PFAssessmentsPicker.Date;
                Assessment.obend = PFAssessmentsEndPicker.Date;
                Assessment.NotificationsEnabled = notificationsSwitch.IsToggled;

                await databaseService.InsertAssessmentAsync(Assessment);
                MessagingCenter.Send(this, "PFAssessmentAdded", Assessment);
            }

            await DisplayAlert("Success", "Assessment saved successfully!", "OK");
            await Navigation.PopAsync();

            if (notificationsSwitch.IsToggled)
            {
                ScheduleNotifications();
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save the assessment: {ex.Message}", "OK");
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
            Title = "Assessment Start Reminder",
            Description = $"Your assessment '{Assessment.PerformanceAssessmentName}' is starting today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = Assessment.Start
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);

        var endNotification = new NotificationRequest
        {
            NotificationId = 1001,
            Title = "Assessment End Reminder",
            Description = $"Your assessment '{Assessment.PerformanceAssessmentName}' is ending today.",
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
            DisplayAlert("Alert", "The notifications for the start and end assessment have been turned on.", "OK");
            ScheduleNotifications();
        }
        else
        {
            DisplayAlert("Alert", "The notifications for the start and end assessment have been turned off.", "OK");
            CancelNotifications();
        }
    }

    private void CancelNotifications()
    {
        LocalNotificationCenter.Current.Cancel(1000);
        LocalNotificationCenter.Current.Cancel(1001);
    }
}

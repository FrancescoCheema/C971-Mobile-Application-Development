using C971.Models;
using C971.Services;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;


namespace C971.Views;
public partial class AddOBAssessment : ContentPage
{
    public int assessmentId { get; set; } 
    public Courses selectedCourse { get; set; }
    public Assessments Assessment { get; set; }

    public static int selectedCourseId;

    public DatabaseService databaseService;
    public Courses Course { get; set; }

    Assessments lastSelection;

    List<Assessments> assessmentList;

    private List<Assessments> allAssessments;
    public ObservableCollection<Assessments> FilteredAssessments { get; set; }
    public AddOBAssessment(Courses course, Assessments assessment)
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

        notificationsSwitch.IsEnabled = true;

        objectiveAssessmentLabel.TextChanged += objectiveAssessmentLabel_TextChanged;

        OBAssessmentsPicker.Date = DateTime.Now;
        OBAssessmentsEndPicker.Date = DateTime.Now;
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
            await DisplayAlert("Missing assessment name", "Please enter a name.", "OK");
            return;
        }

        try
        {
            if (Assessment.CourseId >= 0)
            {
                Assessment.CourseId = selectedCourse.CourseId;
                Assessment.ObjectiveAssessmentName = objectiveAssessmentLabel.Text;
                Assessment.obstart = OBAssessmentsPicker.Date;
                Assessment.obend = OBAssessmentsEndPicker.Date;
                Assessment.NotificationsEnabled = notificationsSwitch.IsToggled;

                await databaseService.InsertAssessmentAsync(Assessment);
                MessagingCenter.Send(this, "AssessmentAdded", Assessment);
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
            Description = $"Your assessment '{Assessment.ObjectiveAssessmentName}' is starting today.",
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
            Description = $"Your assessment '{Assessment.ObjectiveAssessmentName}' is ending today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = Assessment.end
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);
    }

    private void Notifications_Toggled(object sender, ToggledEventArgs e)
    {
        selectedCourse.NotificationsEnabled = e.Value;

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

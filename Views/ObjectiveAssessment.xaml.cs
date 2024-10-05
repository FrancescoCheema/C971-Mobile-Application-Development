using C971.Models;
using C971.Services;
using System.Timers;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C971.Views;

public partial class ObjectiveAssessment : ContentPage
{
    public Courses selectedCourse { get; set; }
    public Assessments selectedAssessment { get; set; }

    private StackLayout lastSelectedStackLayout;

    private ViewCell lastSelectedCell;

    private Color defaultColor = Colors.White;

    private Color selectedColor = Colors.Orange;

    public static int selectedCourseId;

    Assessments lastSelection;

    public DatabaseService databaseService;

    private bool isDoubleTap = false;

    private System.Timers.Timer tapTimer;

    private List<Assessments> allAssessments;
    public ObservableCollection<Assessments> FilteredAssessments { get; set; }
    public ObjectiveAssessment(Courses course)
    {
        InitializeComponent();
        DisplayAlert("Alert", "In order to edit the assessment, please double-click on the assessment.", "OK");

        if (course != null)
        {
            selectedCourseId = course.courseId;
            selectedCourse = course;
        }
        else
        {
            DisplayAlert("Error", "No course data provided.", "OK");
            selectedCourse = new Courses(); 
        }

        selectedAssessment = new Assessments();
        FilteredAssessments = new ObservableCollection<Assessments>();
        allAssessments = new List<Assessments>();
        databaseService = new DatabaseService();

        BindingContext = this;
        tapTimer = new System.Timers.Timer(300);
        tapTimer.Elapsed += TapTimerElapsed;
        tapTimer.AutoReset = false;

        MessagingCenter.Subscribe<AddOBAssessment, Assessments>(this, "AssessmentAdded", (sender, Assessment) =>
        {
            allAssessments.Add(Assessment);
        });

        MessagingCenter.Subscribe<EditOBAssessments, Assessments>(this, "AssessmentAdded", (sender, Assessment) =>
        {
            allAssessments.Add(Assessment);
        });
    }

    private void TapTimerElapsed(object sender, ElapsedEventArgs e)
    {
        isDoubleTap = false;
    }

    private void ScheduleNotifications()
    {
        var startNotification = new NotificationRequest
        {
            NotificationId = 1000,
            Title = "Course Start Reminder",
            Description = $"Your Course '{selectedAssessment.objectiveAssessmentName}' is starting today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = selectedAssessment.Start
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);

        var endNotification = new NotificationRequest
        {
            NotificationId = 1001,
            Title = "Course End Reminder",
            Description = $"Your Course '{selectedAssessment.objectiveAssessmentName}' is ending today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = selectedAssessment.End
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var assessmentList = await databaseService.GetOBAssessmentList(selectedCourseId);

        if (assessmentList != null && assessmentList.Any())
        {
            obassessmentListView.ItemsSource = new ObservableCollection<Assessments>(assessmentList);
        }
        else
        {
            obassessmentListView.ItemsSource = new ObservableCollection<Assessments>();
            await DisplayAlert("No Assessments", "No assessments found for this course.", "OK");
        }

        allAssessments = assessmentList.ToList();
    }

    private void Notifications_Toggled(object sender, ToggledEventArgs e)
    {
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

    private async void AddAssessments_Clicked(object sender, EventArgs e)
    {
        var assessment = selectedAssessment ?? new Assessments();

        await Navigation.PushAsync(page: new AddOBAssessment(selectedCourse, selectedAssessment));
    }
    private async void assessmentListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

        if (e.Item is Assessments tappedAssessment)
        {
            if (isDoubleTap)
            {
                await Navigation.PushAsync(new EditOBAssessments(selectedCourse, tappedAssessment));
            }
            else
            {
                var tappedStackLayout = ((ViewCell)obassessmentListView.TemplatedItems[e.ItemIndex]).View as StackLayout;

                if (lastSelectedStackLayout != null)
                {
                    lastSelectedStackLayout.BackgroundColor = defaultColor;
                }

                tappedStackLayout.BackgroundColor = selectedColor;

                selectedAssessment = tappedAssessment;
                lastSelectedStackLayout = tappedStackLayout;

                obassessmentListView.SelectedItem = null;

                isDoubleTap = true;
                tapTimer.Start();
            }
        }
    }

    private void assessmentListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        obassessmentListView.SelectedItem = null;
    }

    private async void DeleteOBAssessments_Clicked(object sender, EventArgs e)
    {

        if (selectedAssessment != null)
        {
            bool confirm = await DisplayAlert("Delete Assessment", "Are you sure you want to delete this assessment?", "Yes", "No");
            if (confirm)
            {
                await databaseService.DeleteAssessment(selectedAssessment.CourseId, selectedAssessment.assessmentId);

                var assessments = (ObservableCollection<Assessments>)obassessmentListView.ItemsSource;
                assessments.Remove(selectedAssessment);

                lastSelectedStackLayout = null;
                selectedAssessment = null;

                await DisplayAlert("Deleted", "The assessment has been deleted.", "OK");
            }
        }
        else
        {
            await DisplayAlert("No Selection", "Please select an assessment to delete.", "OK");
        }
    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchTerm = e.NewTextValue;
        FilteredAssessments.Clear();


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var filteredList = allAssessments
                .Where(a => a.ObjectiveAssessmentName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var assessment in filteredList)
            {
                FilteredAssessments.Add(assessment);
            }

            obassessmentListView.ItemsSource = FilteredAssessments;
        }
        else
        {
            obassessmentListView.ItemsSource = new ObservableCollection<Assessments>(allAssessments);
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OBAssessmentReport(selectedCourse));
    }
}
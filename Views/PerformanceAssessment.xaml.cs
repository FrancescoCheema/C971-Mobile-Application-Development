using C971.Models;
using C971.Services;
using System.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Graphics; 


namespace C971.Views;

public partial class PerformanceAssessment : ContentPage
{
    public int assessmentId { get; set; }
    public Courses selectedCourse{ get; set; }
    public Assessments selectedAssessment{ get; set; }

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
    public PerformanceAssessment(Courses course)
	{
        InitializeComponent();
        DisplayAlert("Alert", "In order to edit the assessment, please double-click on the assessment.", "OK");
        selectedAssessment = new Assessments();
        FilteredAssessments = new ObservableCollection<Assessments>();
        allAssessments = new List<Assessments>();
        selectedCourseId = course.courseId;
        selectedCourse = course;
        databaseService = new DatabaseService();
        BindingContext = this;

        tapTimer = new System.Timers.Timer(300);
        tapTimer.Elapsed += TapTimerElapsed;
        tapTimer.AutoReset = false;

        MessagingCenter.Subscribe<AddPFAssessments, Assessments>(this, "PFAssessmentAdded", (sender, Assessment) =>
        {
            allAssessments.Add(Assessment);
        });

        MessagingCenter.Subscribe<EditAssessments, Assessments>(this, "AssessmentAdded", (sender, Assessment) =>
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
            Title = "Assessment Start Reminder",
            Description = $"Your assessment '{selectedAssessment.performanceAssessmentName}' is starting today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = selectedCourse.StartCourse
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);

        var endNotification = new NotificationRequest
        {
            NotificationId = 1001,
            Title = "Assessment End Reminder",
            Description = $"Your assessment '{selectedAssessment.performanceAssessmentName}' is ending today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = selectedCourse.EndCourse
            }
        };
        LocalNotificationCenter.Current.Show(startNotification);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var assessmentList = await databaseService.GetAssessmentList(selectedCourseId);

        if (assessmentList != null && assessmentList.Any())
        {
            assessmentListView.ItemsSource = new ObservableCollection<Assessments>(assessmentList);
        }
        else
        {
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

        await Navigation.PushAsync(page: new AddPFAssessments(selectedCourse, selectedAssessment));
    }

    private async void ObjectiveAssessments_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(page: new ObjectiveAssessment(selectedCourse));
    }

    private async void assessmentListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

        if (e.Item is Assessments tappedAssessment)
        {
            if (isDoubleTap)
            {
                await Navigation.PushAsync(new EditAssessments(selectedCourse, tappedAssessment));
            }
            else
            {
                var tappedStackLayout = ((ViewCell)assessmentListView.TemplatedItems[e.ItemIndex]).View as StackLayout;

                if (lastSelectedStackLayout != null)
                {
                    lastSelectedStackLayout.BackgroundColor = defaultColor;
                }

                tappedStackLayout.BackgroundColor = selectedColor;

                selectedAssessment = tappedAssessment;
                lastSelectedStackLayout = tappedStackLayout;

                assessmentListView.SelectedItem = null;

                isDoubleTap = true;
                tapTimer.Start();
            }
        }
    }

    private void assessmentListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        assessmentListView.SelectedItem = null;
    }

    private async void DeleteAssessments_Clicked(object sender, EventArgs e)
    {
        if (selectedAssessment != null)
        {
            bool confirm = await DisplayAlert("Delete Assessment", "Are you sure you want to delete this assessment?", "Yes", "No");
            if (confirm)
            {
                await databaseService.DeleteAssessment(selectedAssessment.CourseId, selectedAssessment.assessmentId);

                var assessments = (ObservableCollection<Assessments>)assessmentListView.ItemsSource;
                assessments.Remove(selectedAssessment);
                allAssessments.Remove(selectedAssessment);
                RefreshListView();

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

    private void RefreshListView()
    {
        assessmentListView.ItemsSource = new ObservableCollection<Assessments>(allAssessments);
    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchTerm = e.NewTextValue;
        FilteredAssessments.Clear();


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var filteredList = allAssessments
                .Where(a => a.PerformanceAssessmentName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var assessment in filteredList)
            {
                FilteredAssessments.Add(assessment);
            }

            assessmentListView.ItemsSource = FilteredAssessments;
        }
        else
        {
            assessmentListView.ItemsSource = new ObservableCollection<Assessments>(allAssessments);
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PFAssessmentReport(selectedCourse));
    }
}
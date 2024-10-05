using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;
using static Android.Graphics.ImageDecoder;

namespace C971.Views;

public partial class ViewTerm : ContentPage
{
    DatabaseService databaseService;

    public Assessments selectedAssessment { get; set; }

    ObservableCollection<Courses> coursesList;

    public Courses selectedCourse;

    public int courseId { get; set; }

    public int _courseId;

    private List<Courses> allCourses;
    public ObservableCollection<Courses> FilteredCourses { get; set; }

    public ViewTerm()
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        FilteredCourses = new ObservableCollection<Courses>();
        allCourses = new List<Courses>();
        _courseId = courseId;
        courseId = 0;
        coursesList = new ObservableCollection<Courses>();
        collectionView.ItemsSource = coursesList;

        MessagingCenter.Subscribe<AddCourse, Courses>(this, "CourseAdded", (sender, newCourse) =>
        {
            if (!allCourses.Any(c => c.CourseId == newCourse.CourseId))
            {
                allCourses.Add(newCourse);
                coursesList.Add(newCourse);
            }
        });


        MessagingCenter.Subscribe<EditCourse, Courses>(this, "CourseAdded", (sender, selectedCourse) =>
        {
            if (!allCourses.Any(c => c.CourseId == selectedCourse.CourseId))
            {
                allCourses.Add(selectedCourse);
            }
        });

        InitializeCoursesAsync();
    }

    Courses lastSelection;
    async void InitializeCoursesAsync()
    {
        await databaseService.Init();

        var existingCourses = await databaseService.GetCourses();
        if (existingCourses.Count == 0)
        {
            await AddDummyCoursesForTermAsync();
            await AddPredefinedAssessments();
            existingCourses = await databaseService.GetCourses();
        }

        await ViewCourseInDatabase();
        collectionView.ItemsSource = null; 
        collectionView.ItemsSource = coursesList;
    }

    async 
    Task
AddDummyCoursesForTermAsync()
    {
        await databaseService.Init();

        var courses = new List<Courses>
    {
        new Courses { CourseId = 1, CourseTitle = "Algebra", StartCourse = new DateTime(2024, 4, 17), EndCourse = new DateTime(2024, 8, 26), CourseStatus = "In progress", InstructorName = "Anika Patel", InstructorPhone = "555-123-4567", InstructorEmail = "anika.patel@strimeuniversity.edu", CourseNotes = "This is an Algebra class." },
        new Courses { CourseId = 2, CourseTitle = "Geography", StartCourse = new DateTime(2024, 1, 25), EndCourse = new DateTime(2024, 10, 19), CourseStatus = "Completed", InstructorName = "Mario Esperanza", InstructorPhone = "789-505-8977", InstructorEmail = "mesperanza@school.com", CourseNotes = "This is a Geography class." },
        new Courses { CourseId = 3, CourseTitle = "English Composition", StartCourse = new DateTime(2024, 3, 5), EndCourse = new DateTime(2024, 9, 5), CourseStatus = "Planning to take", InstructorName = "Gino Paoli", InstructorPhone = "789-083-6784", InstructorEmail = "gpaoli@school.com", CourseNotes = "This is an English Composition class."},
        new Courses { CourseId = 4, CourseTitle = "Biology", StartCourse = new DateTime(2024, 1, 13), EndCourse = new DateTime(2024, 7, 13), CourseStatus = "In progress", InstructorName = "Andrew Flint", InstructorPhone = "789-372-6591", InstructorEmail = "aflint@school.com", CourseNotes = "This is a Biology class." },
        new Courses { CourseId = 5, CourseTitle = "Programming", StartCourse = new DateTime(2024, 8, 8), EndCourse = new DateTime(2025, 2, 8), CourseStatus = "In progress", InstructorName = "Connie Belusha", InstructorPhone = "789-777-8920", InstructorEmail = "cbelusha@school.com", CourseNotes = "This is a Programming class."},
        new Courses { CourseId = 6, CourseTitle = "Philosophy", StartCourse = new DateTime(2024, 5, 9), EndCourse = new DateTime(2024, 11, 9), CourseStatus = "Dropped", InstructorName = "Tao Li", InstructorPhone = "789-231-7356", InstructorEmail = "tli@school.com", CourseNotes = "This is a Philosophy class." }
    };

        foreach (var course in courses)
        {
            await databaseService.AddCourseAsync(course);
        }

        await ViewCourseInDatabase();
        allCourses = coursesList.ToList();
    }

    public async Task AddPredefinedAssessments()
    {
        await databaseService.Init();

        var assessments = new List<Assessments>
    {
        new Assessments { CourseId = 1, PerformanceAssessmentName = "Algebra Performance Assessment", ObjectiveAssessmentName = "Algebra Objective Assessment", ObStart = new DateTime(2024, 8, 1), ObEnd = new DateTime(2024, 8, 31) , Start = new DateTime(2024, 8, 1), End = new DateTime(2024, 8, 31)},
        new Assessments { CourseId = 2, PerformanceAssessmentName = "Geography Performance Assessment", ObjectiveAssessmentName = "Geography Objective Assessment", ObStart = new DateTime(2024, 9, 1), ObEnd = new DateTime(2024, 9, 30), Start = new DateTime(2024, 9, 1), End = new DateTime(2024, 9, 30) },
        new Assessments { CourseId = 3, PerformanceAssessmentName = "English Composition Performance Assessment", ObjectiveAssessmentName = "English Composition Objective Assessment", ObStart = new DateTime(2024, 10, 1), ObEnd = new DateTime(2024, 10, 31), Start = new DateTime(2024, 10, 1), End = new DateTime(2024, 10, 31)},
        new Assessments { CourseId = 4, PerformanceAssessmentName = "Biology Performance Assessment", ObjectiveAssessmentName = "Biology Objective Assessment", ObStart = new DateTime(2024, 11, 1), ObEnd = new DateTime(2024, 11, 30),  Start = new DateTime(2024, 11, 1), End = new DateTime(2024, 11, 30) },
        new Assessments { CourseId = 5, PerformanceAssessmentName = "Programming Performance Assessment", ObjectiveAssessmentName = "Programming Objective Assessment", ObStart = new DateTime(2024, 12, 1), ObEnd = new DateTime(2024, 12, 31), Start = new DateTime(2024, 12, 1), End = new DateTime(2024, 12, 31) },
        new Assessments { CourseId = 6, PerformanceAssessmentName = "Philosophy Performance Assessment", ObjectiveAssessmentName = "Philosophy Objective Assessment", ObStart = new DateTime(2024, 12, 1), ObEnd = new DateTime(2024, 12, 31), Start = new DateTime(2024, 12, 1), End = new DateTime(2024, 12, 31) }
    };

        foreach (var assessment in assessments)
        {
            try
            {
                await databaseService.InsertAssessmentAsync(assessment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting assessment: {ex.Message}");
            }
        }
    }


    async Task ViewCourseInDatabase()
    {
        List<Courses> course = await databaseService.GetCourses();
        foreach (var courses in course)
        {
            coursesList.Add(courses);
        }

        allCourses = course.ToList();
    }


    private void collectionView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {
        lastSelection = e.CurrentSelection.FirstOrDefault() as Courses;
    }

    private async void EditCourse_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            Courses viewModel = new Courses();
            await Navigation.PushAsync(page: new EditCourse(lastSelection, databaseService));
        }
    }

    private async void ViewDetails_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            Courses viewModel = new Courses();
            await Navigation.PushAsync(page: new ViewDetails(lastSelection));
        }
    }

    private async void Assessments_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(page: new PerformanceAssessment(lastSelection));
    }

    private void ShareButton_Clicked(object sender, EventArgs e)
    {

    }


    private async void DeleteCourse_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            bool confirm = await DisplayAlert("Delete Item", $"Are you sure you want to delete {lastSelection.CourseTitle}?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    await databaseService.DeleteCourse(lastSelection.CourseId);  
                    coursesList.Remove(lastSelection); 
                    lastSelection = null;

                    var updatedCourses = await databaseService.GetCourses();
                    coursesList.Clear();
                    foreach (var course in updatedCourses)
                    {
                        coursesList.Add(course);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete the course: {ex.Message}", "OK");
                }
            }
        }
        else
        {
            await DisplayAlert("Error", "No course selected to delete.", "OK");
        }
    }

    private async void AdddCourse_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(page: new AddCourse(databaseService, _courseId, coursesList));
    }

    private async void EditAssessment_Clicked(object sender, EventArgs e)
    {
    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchTerm = e.NewTextValue;
        FilteredCourses.Clear();


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var filteredList = allCourses
                .Where(a => a.CourseTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var course in filteredList)
            {
                FilteredCourses.Add(course);
            }

            collectionView.ItemsSource = FilteredCourses;
        }
        else
        {
            collectionView.ItemsSource = new ObservableCollection<Courses>(allCourses);
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CourseReport(databaseService));
    }
}

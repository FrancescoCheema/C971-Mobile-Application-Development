using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace C971.Views;

public partial class ViewTerm : ContentPage
{
    DatabaseService databaseService;

    ObservableCollection<Courses> coursesList;
    Courses lastSelection;
    public Courses selectedCourse;

    public int courseId { get; set; }
    public int _courseId;

    public ViewTerm()
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        _courseId = courseId;
        coursesList = new ObservableCollection<Courses>();
        collectionView.ItemsSource = coursesList;
        AddDummyCoursesForTermAsync();
        ViewCourseInDatabase();
    }

    async void InitializeCoursesAsync()
    {
        await databaseService.Init();

        var existingCourses = await databaseService.GetCourses();
        if (existingCourses.Count == 0)
        {
            AddDummyCoursesForTermAsync();
        }

        await ViewCourseInDatabase();
    }

    async void AddDummyCoursesForTermAsync()
    {
        await databaseService.Init();
        List<Courses> courses = new List<Courses>
            {
                new Courses { CourseTitle = "Algebra", StartCourse = new DateTime(2024, 4, 17), EndCourse = new DateTime(2024, 8, 26), CourseStatus = "In progress", InstructorName = "Anika Patel", InstructorPhone = "555-123-4567", InstructorEmail = "anika.patel@strimeuniversity.edu", CourseNotes = "this is an Algebra class." },
                new Courses { CourseTitle = "Geography", StartCourse = new DateTime(2024, 1, 25), EndCourse = new DateTime(2024, 10, 19), CourseStatus = "Completed", InstructorName = "Mario Esperanza", InstructorPhone = "789-505-8977", InstructorEmail = "mesperanza@school.com", CourseNotes = "this is a Geography class." },
                new Courses { CourseTitle = "English Composition", StartCourse = new DateTime(2024, 3, 5), EndCourse = new DateTime(2024, 9, 5), CourseStatus = "Planning to take", InstructorName = "Gino Paoli", InstructorPhone = "789-083-6784", InstructorEmail = "gpaoli@school.com", CourseNotes = "this is an English Composition class."},
                new Courses { CourseTitle = "Biology", StartCourse = new DateTime(2024, 1, 13), EndCourse = new DateTime(2024, 7, 13), CourseStatus = "In progress", InstructorName = "Andrew Flint", InstructorPhone = "789-372-6591", InstructorEmail = "aflint@school.com", CourseNotes = "this is a Biology class." },
                new Courses { CourseTitle = "Programming", StartCourse = new DateTime(2024, 8, 8), EndCourse = new DateTime(2025, 2, 8), CourseStatus = "In progress", InstructorName = "Connie Belusha", InstructorPhone = "789-777-8920", InstructorEmail = "cbelusha@school.com", CourseNotes = "this is a Programming class."},
                new Courses { CourseTitle = "Philosophy", StartCourse = new DateTime(2024, 5, 9), EndCourse = new DateTime(2024, 11, 9), CourseStatus = "Dropped", InstructorName = "Tao Li", InstructorPhone = "789-231-7356", InstructorEmail = "tli@school.com", CourseNotes = "this is a Philosophy class." },
            };

        foreach (var course in courses)
        {
            await databaseService.AddCourseAsync(course);
        }

        await ViewCourseInDatabase();
    }


    async Task ViewCourseInDatabase()
    {
        List<Courses> course = await databaseService.GetCourses();
        foreach (var courses in course)
        {
            coursesList.Add(courses);
        }
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
        await Navigation.PushAsync(page: new Assessments(lastSelection));
    }

    private void ShareButton_Clicked(object sender, EventArgs e)
    {

    }


    private async void DeleteCourse_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            bool confirm = await DisplayAlert("Delete Item", "Are you sure that you want to delete this course?", "Yes", "No");
            if (confirm)
            {
                await databaseService.DeleteCourse(lastSelection.CourseId);
                coursesList.Remove(lastSelection);
                lastSelection = null;
                await databaseService.GetCourses();
            }
        }
    }

    private async void AdddCourse_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(page: new AddCourse(databaseService, _courseId, coursesList));
    }

}

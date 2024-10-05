using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C971.Services;
using C971.Models;
using System.Collections.ObjectModel;

namespace C971.Views;

public partial class PFAssessmentReport : ContentPage
{
    private readonly DatabaseService _databaseService;

    private readonly int _selectedCourseId;

    public ObservableCollection<Assessments> assessmentList { get; set; }

    public PFAssessmentReport(Courses selectedCourse)
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        assessmentList = new ObservableCollection<Assessments>();
        _selectedCourseId = selectedCourse.courseId;

        BindingContext = this;

        GenerateReport();
    }

    private async Task GenerateReport()
    {
        await _databaseService.Init();
        List<Assessments> assessments = await _databaseService.GetAssessments();

        var filteredAssessments = assessments
            .Where(a => a.CourseId == _selectedCourseId)
            .ToList();

        assessmentList.Clear();
        foreach (var assessment in filteredAssessments)
        {
            assessmentList.Add(assessment);
        }
    }

    private async void GenerateReportButton_Clicked(object sender, EventArgs e)
    {
        await GenerateReport();
        await DisplayAlert("Report Generated", "The report has been generated successfully.", "OK");
    }
}
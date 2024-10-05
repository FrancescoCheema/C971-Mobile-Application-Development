using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C971.Services;
using C971.Models;
using System.Collections.ObjectModel;

namespace C971;

public partial class OBAssessmentReport : ContentPage
{
    private readonly DatabaseService _databaseService;

    private readonly int _selectedCourseId;

    public ObservableCollection<Assessments> obassessmentList { get; set; }

    public OBAssessmentReport(Courses selectedCourse)
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        obassessmentList = new ObservableCollection<Assessments>();
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

        obassessmentList.Clear();
        foreach (var assessment in filteredAssessments)
        {
            obassessmentList.Add(assessment);
        }
    }

    private async void GenerateReportButton_Clicked(object sender, EventArgs e)
    {
        await GenerateReport();
        await DisplayAlert("Report Generated", "The report has been generated successfully.", "OK");
    }
}
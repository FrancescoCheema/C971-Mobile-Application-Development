using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C971.Services;
using C971.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace C971.Views;

public partial class CourseReport : ContentPage
{
    private readonly DatabaseService _databaseService;
    public int courseId { get; set; }

    public ObservableCollection<Courses> CourseList { get; set; }
    public ObservableCollection<Assessments> AssessmentList { get; set; }

    public CourseReport(DatabaseService dbService)
    {
        InitializeComponent();
        _databaseService = dbService;
        CourseList = new ObservableCollection<Courses>();
        collectionView.ItemsSource = CourseList;

        MessagingCenter.Subscribe<AddCourse, Courses>(this, "CourseAdded", async (sender, newCourse) =>
        {
            await GenerateReport();
        });

        GenerateReport();
    }

    private async Task GenerateReport()
    {
        var courses = await _databaseService.GetCourses();
        CourseList.Clear();  
        foreach (var course in courses)
        {
            CourseList.Add(course); 
        }
    }
}

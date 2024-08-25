using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using SQLitePCL;

namespace C971.Views;

public partial class ViewDetails : ContentPage
{
    public Courses selectedCourse { get; set; }
    List<Courses> coursesList;
    public ViewDetails(Courses course)
	{
		InitializeComponent();

        selectedCourse = course;
        BindingContext = selectedCourse;

    }


    private void ShareButton_Clicked(object sender, EventArgs e)
    {

    }
}
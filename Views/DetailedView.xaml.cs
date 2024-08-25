using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using SQLitePCL;

namespace C971.Views;

public partial class DetailedView : ContentPage
{
    DatabaseService databaseService;
    List<Terms> termsList;
    public Terms selectedTerm { get; set; }
    public DetailedView(Terms term)
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        selectedTerm = term;
        BindingContext = selectedTerm;
    }
}
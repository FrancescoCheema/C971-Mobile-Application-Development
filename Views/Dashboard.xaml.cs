using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using SQLitePCL;
using System.Collections.ObjectModel;

namespace C971.Views;

public partial class Dashboard : ContentPage
{
    DatabaseService databaseService;
    ObservableCollection<Terms> termList;
    public Terms selectedTerm;
    int termId;
    public Dashboard()
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        termList = new ObservableCollection<Terms>();
        collectionView.ItemsSource = termList;
        AddTermsToDatabase();
        ViewTermInDatabase();
    }

    async void AddTermsToDatabase()
    {
        await databaseService.Init();
        List<Terms> terms = new List<Terms>
            {
                new Terms { termTitle = "Term 1", start = new DateTime(2024, 2, 25), end = new DateTime(2024, 8, 25) },
                new Terms { termTitle = "Term 2", start = new DateTime(2024, 4, 30), end = new DateTime(2024, 10, 30) },
                new Terms { termTitle = "Spring Term", start = new DateTime(2024, 1, 10), end = new DateTime(2024, 7, 10) },
                new Terms { termTitle = "Summer Term", start = new DateTime(2024, 3, 14), end = new DateTime(2024, 9, 14) },
            };

        foreach (var term in terms)
        {
            await databaseService.AddTermAsync(term);

        }
    }

    async Task ViewTermInDatabase()
    {
        List<Terms> terms = await databaseService.GetTerms();
        foreach (var term in terms)
        {
            termList.Add(term);
        }
    }

    Terms lastSelection;

    private void Terms_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        lastSelection = e.CurrentSelection.FirstOrDefault() as Terms;
    }

    async void ViewTerm_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            await Navigation.PushAsync(page: new ViewTerm());
        }
    }

    private void EditTerm_Clicked(object sender, EventArgs e)
    {
    }

    private async void DetailedView_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            Terms viewModel = new Terms();
            await Navigation.PushAsync(page: new DetailedView(lastSelection));
        }
    }
}


using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using static Android.Graphics.ImageDecoder;

namespace C971.Views;

public partial class Dashboard : ContentPage
{
    DatabaseService databaseService;
    ObservableCollection<Terms> termList;
    public Terms selectedTerm;
    public int termId { get; set; }
    int _termId;

    public List<Terms> allTerms;
    public ObservableCollection<Terms> FilteredTerms { get; set; }
    public Dashboard()
    {
        InitializeComponent();
        databaseService = new DatabaseService();
        FilteredTerms = new ObservableCollection<Terms>();
        allTerms = new List<Terms>();
        termList = new ObservableCollection<Terms>();
        _termId = termId;
        collectionView.ItemsSource = termList;
        MessagingCenter.Subscribe<AddTerm, Terms>(this, "TermAdded", (sender, newTerm) =>
        {
            allTerms.Add(newTerm);
        });

        MessagingCenter.Subscribe<EditTerm, Terms>(this, "TermAdded", (sender, selectedTerm) =>
        {
            allTerms.Add(selectedTerm);
        });

        MessagingCenter.Subscribe<ViewTerm, Courses>(this, "CourseAdded", async (sender, newCourse) =>
        {
            InitializeTermsAsync(); 
        });

        InitializeTermsAsync();
    }

    async 
    Task
AddTermsToDatabase()
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

    async void InitializeTermsAsync()
    {
        await databaseService.Init();

        var existingTerms = await databaseService.GetTerms();
        if (existingTerms.Count == 0)
        {
            await AddTermsToDatabase();
            existingTerms = await databaseService.GetTerms(); 
        }

        allTerms = existingTerms.ToList(); 

        await ViewTermInDatabase();
    }

    async Task ViewTermInDatabase()
    {
        termList.Clear();
        List<Terms> terms = await databaseService.GetTerms();
        foreach (var term in terms)
        {
            termList.Add(term);
        }

        allTerms = terms.ToList();
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

    private async void EditTerm_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            Terms viewModel = new Terms();
            await Navigation.PushAsync(page: new EditTerm(lastSelection, databaseService, OnTermEdited));
        }
    }

    private async void DetailedView_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            Terms viewModel = new Terms();
            await Navigation.PushAsync(page: new DetailedView(lastSelection));
        }
    }

    private async Task OnTermEdited()
    {
        await ViewTermInDatabase();
    }

    private async void AddTerm_Clicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(page: new AddTerm(lastSelection, databaseService, termList));
    }

    private void EditAssessments_Clicked(object sender, EventArgs e)
    {

    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchTerm = e.NewTextValue;
        FilteredTerms.Clear();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var filteredList = allTerms
                .Where(a => a.TermTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var term in filteredList)
            {
                FilteredTerms.Add(term);
            }

            collectionView.ItemsSource = FilteredTerms;  
        }
        else
        {
            collectionView.ItemsSource = new ObservableCollection<Terms>(allTerms);  
        }
    }

    private async void Reports_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReportPage());
    }

    private async void DeleteTerm_Clicked(object sender, EventArgs e)
    {
        if (lastSelection != null)
        {
            bool confirm = await DisplayAlert("Delete Item", "Are you sure that you want to delete this term?", "Yes", "No");
            if (confirm)
            {
                await databaseService.DeleteTerm(lastSelection.termId);
                termList.Remove(lastSelection);
                lastSelection = null;
                await databaseService.GetCourses();
            }
        }
    }
}


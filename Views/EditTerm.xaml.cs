using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;

namespace C971.Views;

public partial class EditTerm : ContentPage
{
    public Terms selectedTerm { get; set; }
    public DatabaseService databaseService;
    Terms lastSelection;
    public Terms terms;
    List<Terms> termList = new List<Terms>();
    private Action _onTermUpdated;
    private readonly Func<Task> refreshCallback;

    public EditTerm(Terms term, DatabaseService dbService, Func<Task> refreshCallback)
    {
        InitializeComponent();
        terms = term;
        selectedTerm = term;
        databaseService = dbService;
        this.refreshCallback = refreshCallback;
        BindingContext = selectedTerm;


        termTitleLabel.TextChanged += TermTitleLabel_TextChanged;
        termPicker.DateSelected += Picker_SelectedIndexChanged;
        termEndPicker.DateSelected += Picker_SelectedIndexChanged;
    }

    private void TermTitleLabel_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckForm();
    }

    private void Picker_SelectedIndexChanged(object sender, DateChangedEventArgs e)
    {
        CheckForm();
    }

    private async Task ViewTermInDatabase()
    {
        termList.Clear();
        List<Terms> terms = await databaseService.GetTerms();
        foreach (var term in terms)
        {
            termList.Add(term);
        }
    }

    private async void CheckForm()
    {
        if (string.IsNullOrEmpty(termTitleLabel.Text))
        {
            await DisplayAlert("Missing term name", "Please Enter a Term Name", "Ok");
            saveTermButton.IsEnabled = false;
        }
        else
        {
            saveTermButton.IsEnabled = true;
        }
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(termTitleLabel.Text))
        {
            await DisplayAlert("Missing term name", "Please Enter a Term Name", "Ok");
            saveTermButton.IsEnabled = false;
            return;
        }
        else
        {
            saveTermButton.IsEnabled = true;

            terms.termTitle = termTitleLabel.Text;
            terms.start = termPicker.Date;
            terms.end = termEndPicker.Date;

            await databaseService.UpdateTermAsync(selectedTerm);
            MessagingCenter.Send(this, "TermAdded", selectedTerm);
            await DisplayAlert("Success", "Term saved successfully!", "OK");
            await Navigation.PopAsync();

            if (refreshCallback != null)
            {
                await refreshCallback();
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        lastSelection = e.CurrentSelection.FirstOrDefault() as Terms;
    }
}
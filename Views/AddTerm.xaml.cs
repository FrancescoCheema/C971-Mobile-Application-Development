using C971.Models;
using C971.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace C971.Views;

public partial class AddTerm : ContentPage
{
    public Terms selectedTerm { get; set; }
    public DatabaseService databaseService;
    Terms lastSelection;
    public Terms terms;
    private Action _onTermUpdated;
    private ObservableCollection<Terms> _termList;

    private DateTime _startTermDate;
    public DateTime StartTermDate
    {
        get => _startTermDate;
        set
        {
            if (_startTermDate != value)
            {
                _startTermDate = value;
                EndTermDate = _startTermDate.AddMonths(1);
                OnPropertyChanged(nameof(StartTermDate));
            }
        }
    }

    private DateTime _endTermDate;
    public DateTime EndTermDate
    {
        get => _endTermDate;
        set
        {
            if (_endTermDate != value)
            {
                _endTermDate = value;
                OnPropertyChanged(nameof(EndTermDate));
            }
        }
    }

    public AddTerm(Terms term, DatabaseService dbService, ObservableCollection<Terms> termList)
    {
        InitializeComponent();
        terms = term;
        _termList = termList;
        databaseService = dbService;
        StartTermDate = DateTime.Now;
        EndTermDate = StartTermDate.AddMonths(1);
        BindingContext = this;


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
        _termList.Clear();
        List<Terms> terms = await databaseService.GetTerms();
        foreach (var term in terms)
        {
            _termList.Add(term);
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

            Terms newTerm = new Terms
            {
                termTitle = termTitleLabel.Text,
                start = termPicker.Date,
                end = termEndPicker.Date
            };

                await databaseService.AddTermAsync(newTerm);
                _termList.Add(newTerm);
                await DisplayAlert("Success", "Term saved successfully!", "OK");
                MessagingCenter.Send(this, "TermAdded", newTerm);
                await Navigation.PopAsync();
                ViewTermInDatabase();
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
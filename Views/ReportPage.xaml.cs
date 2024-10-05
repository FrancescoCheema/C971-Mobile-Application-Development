using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C971.Services;
using C971.Models;
using System.Collections.ObjectModel;

namespace C971.Views
{
    public partial class ReportPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Terms> TermsList { get; set; }

        public ReportPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            TermsList = new ObservableCollection<Terms>();

            BindingContext = this; 

            GenerateReport();
        }

        private async Task GenerateReport()
        {
            await _databaseService.Init();
            List<Terms> terms = await _databaseService.GetTerms();

            TermsList.Clear();
            foreach (var term in terms)
            {
                TermsList.Add(term);
            }
        }

        private async void GenerateReportButton_Clicked(object sender, EventArgs e)
        {
            await GenerateReport();
            await DisplayAlert("Report Generated", "The report has been generated successfully.", "OK");
        }
    }
}
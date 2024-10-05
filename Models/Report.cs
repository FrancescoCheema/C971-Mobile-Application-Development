using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.Services;
using C971.Models;
using Microsoft.Maui.Controls;

namespace C971.Models;

public class TermReport
{
    private List<Terms> _terms;

    public TermReport(List<Terms> terms)
    {
        _terms = terms;
    }

    public string GetReportContent()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Terms Report");
        sb.AppendLine("--------------");

        foreach (var term in _terms)
        {
            sb.AppendLine($"Term Title: {term.TermTitle}");
            sb.AppendLine($"Start Date: {term.Start.ToShortDateString()}");
            sb.AppendLine($"End Date: {term.End.ToShortDateString()}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Models;
using DbContext;
using Microsoft.EntityFrameworkCore;
using Services;
using AppStudies.Pages;

namespace AppStudies.Pages
{
    //Demonstrate how to use the model to present a list of objects
    //https://www.learnrazorpages.com/razor-pages/forms/checkboxes

    public class CheckboxListModel : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<CheckboxListModel> _logger = null;

        public class csFamousQuoteChkd : csFamousQuote
        {
            public bool Checked { get; set; }
            public csFamousQuoteChkd(csFamousQuote original) : base(original) { }
            public csFamousQuoteChkd() { }
        }

        [BindProperty]
        public List<csFamousQuoteChkd> Quotes1 { get; set; } = new List<csFamousQuoteChkd>();

        [BindProperty]
        public List<csFamousQuoteChkd> Quotes2 { get; set; } = new List<csFamousQuoteChkd>();


        public List<csFamousQuote> SelectedQuotes1 { get; set; } = new List<csFamousQuote>();
        public List<csFamousQuote> SelectedQuotes2 { get; set; } = new List<csFamousQuote>();

        //Will execute on a Get request
        public IActionResult OnGet()
        {
            //Use the Service
            Quotes1 = _service.ReadQuotes().Take(3).Select(q => new csFamousQuoteChkd(q) { Checked = false }).ToList();
            Quotes2 = _service.ReadQuotes().TakeLast(3).Select(q => new csFamousQuoteChkd(q) { Checked = false }).ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            //Page is rendered as the postback is part of the form tag
            //Get only the checked quotes
            var selQuoteIds1 = Quotes1.Where(q => q.Checked).Select(q => q.QuoteId).ToList();
            var selQuoteIds2 = Quotes2.Where(q => q.Checked).Select(q => q.QuoteId).ToList();

            //use the service to find the quote according to selected quote id
            var _quotes = _service.ReadQuotes();

            //Get the checked quotes
            SelectedQuotes1 = _quotes.Where(q => selQuoteIds1.Contains(q.QuoteId)).ToList();
            SelectedQuotes2 = _quotes.Where(q => selQuoteIds2.Contains(q.QuoteId)).ToList();


            return Page();
        }

        //Inject services just like in WebApi
        public CheckboxListModel(IQuoteService service, ILogger<CheckboxListModel> logger)
        {
            _logger = logger;
            _service = service;
        }
    }
}

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

    public class ButtonClickListModel : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<ButtonClickListModel> _logger = null;

        public csFamousQuote SelectedQuote { get; set; } = null;

        [BindProperty]
        public List<csFamousQuote> Quotes1 { get; set; } = new List<csFamousQuote>();

        [BindProperty]
        public List<csFamousQuote> Quotes2 { get; set; } = new List<csFamousQuote>();


        //Will execute on a Get request
        public IActionResult OnGet()
        {
            //Use the Service
            Quotes1 = _service.ReadQuotes().Take(3).ToList();
            Quotes2 = _service.ReadQuotes().TakeLast(3).ToList();

            return Page();
        }

        public IActionResult OnPost(Guid quoteId)
        {
            //Page is rendered as the postback is part of the form tag
            SelectedQuote = _service.ReadQuote(quoteId);

            return Page();
        }

        //Inject services just like in WebApi
        public ButtonClickListModel(IQuoteService service, ILogger<ButtonClickListModel> logger)
        {
            _logger = logger;
            _service = service;
        }
    }
}

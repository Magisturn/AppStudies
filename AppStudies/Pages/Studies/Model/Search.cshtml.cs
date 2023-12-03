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
    public class SearchModel : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<SearchModel> _logger = null;

        //public member becomes part of the Model in the Razor page
        public List<csFamousQuote> Quotes { get; set; } = new List<csFamousQuote>();

        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 5;

        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;

        //ModelBinding for the form
        [BindProperty]
        public string SearchFilter { get; set; } = null;


        //Will execute on a Get request
        public IActionResult OnGet()
        {
            //Read a QueryParameters
            if (int.TryParse(Request.Query["pagenr"], out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }

            SearchFilter = Request.Query["search"];

            //Pagination
            UpdatePagination();

            //Use the Service
            Quotes = _service.ReadQuotes(ThisPageNr, PageSize, SearchFilter);

            return Page();
        }

        private void UpdatePagination()
        {
            //Pagination
            NrOfPages = (int)Math.Ceiling((double)_service.NrOfQuotes(SearchFilter) / PageSize);
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            PresentPages = Math.Min(3, NrOfPages);
        }

        public IActionResult OnPostSearch()
        {
            //Pagination
            UpdatePagination();

            //Use the Service
            Quotes = _service.ReadQuotes(ThisPageNr, PageSize, SearchFilter);

            //Page is rendered as the postback is part of the form tag
            return Page();
        }

        //Inject services just like in WebApi
        public SearchModel(IQuoteService service, ILogger<SearchModel> logger)
        {
            _logger = logger;
            _service = service;
        }
    }
}

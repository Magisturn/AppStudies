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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppStudies.Pages
{
    //Demonstrate how to use the model to present a list of objects
    public enum enAnimals { Zebra, Tiger, Lion, Elephant }

    public class EnumPickModel : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<EnumPickModel> _logger = null;


        //ModelBinding for Selections
        [BindProperty]
        public enAnimals? SelectedAnimal1 { get; set; } = null;
        [BindProperty]
        public enAnimals? SelectedAnimal2 { get; set; } = null;
        [BindProperty]
        public List<enAnimals> SelectedAnimal3 { get; set; } = new List<enAnimals>();


        //Will execute on a Get request
        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            //Page is rendered as the postback is part of the form tag
            return Page();
        }


        //Inject services just like in WebApi
        public EnumPickModel(IQuoteService service, ILogger<EnumPickModel> logger)
        {
            _logger = logger;
            _service = service;
        }
    }
}

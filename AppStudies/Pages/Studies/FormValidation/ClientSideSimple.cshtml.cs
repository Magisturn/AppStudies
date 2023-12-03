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
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppStudies.Pages
{
    //Demonstrate how to read Query parameters
    public class ClientSideSimple : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<ClientSideSimple> _logger = null;

        [BindProperty]
        public csFamousQuoteIM QuoteIM { get; set; }

        public string PageHeader { get; set; }

        //public member becomes part of the Model in the Razor page
        public string ErrorMessage { get; set; } = null;


        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }


        //Will execute on a Get request
        public IActionResult OnGet()
        {
            try
            {
                if (Guid.TryParse(Request.Query["id"], out Guid _id))
                {
                    //Use the Service and populate the InputModel
                    QuoteIM = new csFamousQuoteIM(_service.ReadQuote(_id));
                    PageHeader = "Edit details of a quote";
                }
                else
                {
                    //Create an empty InputModel
                    QuoteIM = new csFamousQuoteIM();
                    QuoteIM.StatusIM = enStatusIM.Inserted;
                    PageHeader = "Create a new quote";
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }

        public IActionResult OnPostUndo()
        {
            //Use the Service and populate the InputModel
            QuoteIM = new csFamousQuoteIM(_service.ReadQuote(QuoteIM.QuoteId));          
            PageHeader = "Edit details of a quote";
            return Page();
        }

        public IActionResult OnPostSave()
        {
            //PageHeader is stored in TempData which has to be set after a Post
            PageHeader = (QuoteIM.StatusIM == enStatusIM.Inserted) ?
                "Create a new quote" : "Edit details of a quote";

            //I use IsValid() instead of ModelState.IsValid in order to extract
            //error information
            if (!IsValid())
            {
                //The page is not valid
                return Page();
            }

            if (QuoteIM.StatusIM == enStatusIM.Inserted)
            {
                //It is an create
                var model = QuoteIM.UpdateModel(new csFamousQuote());
                model = _service.CreateQuote(model);
                QuoteIM = new csFamousQuoteIM(model);
            }
            else
            {
                //It is an update
                //Get orginal
                var model = _service.ReadQuote(QuoteIM.QuoteId);

                //update the changes and save
                model = QuoteIM.UpdateModel(model);
                model = _service.UpdateQuote(model);
                QuoteIM = new csFamousQuoteIM(model);
            }

            return Redirect($"~/Studies/Model/Search?search={QuoteIM.Author}");
        }


        //Inject services just like in WebApi
        public ClientSideSimple(IQuoteService service, ILogger<ClientSideSimple> logger)
        {
            _logger = logger;
            _service = service;
        }

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class csFamousQuoteIM
        {
            //Status of InputModel
            public enStatusIM StatusIM { get; set; }

            //Properties from Model which is to be edited in the <form>
            public Guid QuoteId { get; init; } = Guid.NewGuid();

            [Required(ErrorMessage = "You type provide a quote")]
            public string Quote { get; set; }

            [Required(ErrorMessage = "You must provide an author")]
            public string Author { get; set; }

            #region constructors and model update
            public csFamousQuoteIM() { StatusIM = enStatusIM.Unchanged; }

            //Copy constructor
            public csFamousQuoteIM(csFamousQuoteIM original)
            {
                StatusIM = original.StatusIM;

                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;
            }

            //Model => InputModel constructor
            public csFamousQuoteIM(csFamousQuote original)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;
            }

            //InputModel => Model
            public csFamousQuote UpdateModel(csFamousQuote model)
            {
                model.QuoteId = QuoteId;
                model.Quote = Quote;
                model.Author = Author;
                return model;
            }
            #endregion

        }
        #endregion

        #region Server Side Validation
        private bool IsValid(string[] validateOnlyKeys = null)
        {
            InvalidKeys = ModelState
               .Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                InvalidKeys = InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }

            ValidationErrorMsgs = InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            HasValidationErrors = InvalidKeys.Count() != 0;

            return !HasValidationErrors;
        }
        #endregion
    }
}
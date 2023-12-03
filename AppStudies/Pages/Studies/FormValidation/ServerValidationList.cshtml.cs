using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Newtonsoft.Json;
using Services;

namespace AppStudies.Pages
{
    public class ServerValidationList : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<ServerValidationList> _logger = null;

        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        [BindProperty]
        public List<csFamousQuoteIM> QuotesIM { get; set; }

        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }

        #region HTTP Requests
        public IActionResult OnGet()
        {
            QuotesIM = _service.ReadQuotes().Select(q => new csFamousQuoteIM(q)).ToList();
            return Page();
        }

        public IActionResult OnPostDelete(Guid quoteId)
        {
            //Set the Quote as deleted, it will not be rendered
            QuotesIM.First(q => q.QuoteId == quoteId).StatusIM = enStatusIM.Deleted;
            return Page();
        }

        public IActionResult OnPostEdit(Guid quoteId)
        {
            int idx = QuotesIM.FindIndex(q => q.QuoteId == quoteId);
            string[] keys = { $"QuotesIM[{idx}].editQuote",
                            $"QuotesIM[{idx}].editAuthor"};
            if (!IsValid(keys))
            {
                return Page();
            }

            //Set the Quote as Modified, it will later be updated in the database
            var q = QuotesIM.First(q => q.QuoteId == quoteId);
            q.StatusIM = enStatusIM.Modified;

            //Implement the changes
            q.Author = q.editAuthor;
            q.Quote = q.editQuote;
            return Page();
        }

        public IActionResult OnPostUndo()
        {
            //Reload the InputModel
            QuotesIM = _service.ReadQuotes().Select(q => new csFamousQuoteIM(q)).ToList();
            return Page();
        }

        public IActionResult OnPostSave()
        {
            //Note: Here I will not do any validation as it is done during the OnPostEdit

            //Check if there are deleted quotes, if so simply remove them
            var _deletes = QuotesIM.FindAll(q => (q.StatusIM == enStatusIM.Deleted));
            foreach (var item in _deletes)
            {
                //Remove from the database
                _service.DeleteQuote(item.QuoteId);
            }

            //Check if there are any modified quotes , if so update them in the database
            var _modyfies = QuotesIM.FindAll(a => (a.StatusIM == enStatusIM.Modified));
            foreach (var item in _modyfies)
            {
                //get model
                var model = _service.ReadQuote(item.QuoteId);

                //update the changes and save
                model = item.UpdateModel(model);
                model = _service.UpdateQuote(model);
            }

            //Reload the InputModel
            QuotesIM = _service.ReadQuotes().Select(q => new csFamousQuoteIM(q)).ToList();
            return Page();
        }
        #endregion

        #region Constructors
        //Inject services just like in WebApi
        public ServerValidationList(IQuoteService service, ILogger<ServerValidationList> logger)
        {
            _service = service;
            _logger = logger;
        }
        #endregion

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted}

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

            //Added properites to edit in the list with undo
            [Required(ErrorMessage = "You must provide an quote")]
            public string editQuote { get; set; }

            [Required(ErrorMessage = "You must provide an author")]
            public string editAuthor { get; set; }

            #region constructors and model update
            public csFamousQuoteIM() { StatusIM = enStatusIM.Unchanged; }

            //Copy constructor
            public csFamousQuoteIM(csFamousQuoteIM original)
            {
                StatusIM = original.StatusIM;

                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;

                editQuote = original.editQuote;
                editAuthor = original.editAuthor;
            }

            //Model => InputModel constructor
            public csFamousQuoteIM(csFamousQuote original)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = original.QuoteId;
                Quote = editQuote = original.Quote;
                Author = editAuthor = original.Author;
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

﻿using System;
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
using Models;
using Newtonsoft.Json;
using Services;

namespace AppStudies.Pages
{
    public class InputModelListAddModel : PageModel
    {
        //Just like for WebApi
        IQuoteService _service = null;
        ILogger<InputModelListAddModel> _logger = null;

        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        [BindProperty]
        public List<csFamousQuoteIM> QuotesIM { get; set; }

        [BindProperty]
        public csFamousQuoteIM NewQuoteIM { get; set; } = new csFamousQuoteIM();

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
            //Set the Quote as Modified, it will later be updated in the database
            var q = QuotesIM.First(q => q.QuoteId == quoteId);
            q.StatusIM = enStatusIM.Modified;

            //Implement the changes
            q.Author = q.editAuthor;
            q.Quote = q.editQuote;
            return Page();
        }

        public IActionResult OnPostAdd()
        {
            //Set the Artist as Inserted, it will later be inserted in the database
            NewQuoteIM.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            NewQuoteIM.QuoteId = Guid.NewGuid();

            //Add it to the Input Models artists
            QuotesIM.Add(new csFamousQuoteIM(NewQuoteIM));

            //Clear the NewArtist so another album can be added
            NewQuoteIM = new csFamousQuoteIM();

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
            //Check if there are deleted quotes, if so simply remove them
            var _deletes = QuotesIM.FindAll(q => (q.StatusIM == enStatusIM.Deleted));
            foreach (var item in _deletes)
            {
                //Remove from the database
                _service.DeleteQuote(item.QuoteId);
            }

            #region Add quotes
            //Check if there are any new quotes added, if so create them in the database
            var _newies = QuotesIM.FindAll(q => (q.StatusIM == enStatusIM.Inserted));
            foreach (var item in _newies)
            {
                //Create the corresposning model
                var model = item.UpdateModel(new csFamousQuote());

                //create in the database
                model = _service.CreateQuote(model);
            }
            #endregion

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
        public InputModelListAddModel(IQuoteService service, ILogger<InputModelListAddModel> logger)
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
            public Guid QuoteId { get; set; } = Guid.NewGuid();
            public string Quote { get; set; }
            public string Author { get; set; }

            //Added properites to edit in the list with undo
            public string editQuote { get; set; }
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
    }
}

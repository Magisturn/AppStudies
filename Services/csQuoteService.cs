using Microsoft.EntityFrameworkCore;
using DbContext;
using Models;
using System.Linq;

namespace Services;


public class csQuoteService : IQuoteService
{
    List<csFamousQuote> _quotes = new csSeedGenerator().AllQuotes.Select(q => new csFamousQuote(){Author = q.Author, Quote = q.Quote}).ToList();

    #region CRUD 
    public int NrOfQuotes(string filter)
    {
        filter ??= "";
        return _quotes.Count(q =>
            q.Quote.ToLower().Contains(filter.ToLower()) ||
            q.Author.ToLower().Contains(filter.ToLower()));
    }

    public List<csFamousQuote> ReadQuotes(int? pageNumber, int? pageSize, string filter)
    {
        filter ??= "";
        if (pageNumber == null || pageSize == null)
        {
            return _quotes.Where(q =>
                                q.Quote.ToLower().Contains(filter.ToLower()) ||
                                q.Author.ToLower().Contains(filter.ToLower()))
                          .Select(q => new csFamousQuote(q)).ToList();
        }

        return _quotes.Where(q =>
                                q.Quote.ToLower().Contains(filter.ToLower()) ||
                                q.Author.ToLower().Contains(filter.ToLower()))
                          .Select(q => new csFamousQuote(q))

            //Adding paging
            .Skip(pageNumber.Value * pageSize.Value)
            .Take(pageSize.Value).ToList();
    }


    public csFamousQuote ReadQuote(Guid id)
    {
        var q = _quotes.FirstOrDefault(q => q.QuoteId == id);
        if (q != null)
        {
            return new csFamousQuote(q);
        }
        
        return null;
    }

    public csFamousQuote UpdateQuote(csFamousQuote _src)
    {
        var q = _quotes.FirstOrDefault(q => q.QuoteId == _src.QuoteId);
        if (q != null)
        {
            q.Quote = _src.Quote;
            q.Author = _src.Author;
            return new csFamousQuote(q);
        }
        return null;
    }

    public csFamousQuote CreateQuote(csFamousQuote _src)
    {
       _quotes.Add(_src); 
       return new csFamousQuote(_src);
    }

    public csFamousQuote DeleteQuote(Guid id)
    {
        var q = _quotes.FirstOrDefault(q => q.QuoteId == id);

        //If the item does not exists
        if (q == null) throw new ArgumentException($"Item {id} is not existing");

        _quotes.Remove(q);
        return q;
    }

    #endregion
}




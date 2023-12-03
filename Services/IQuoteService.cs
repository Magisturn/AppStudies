using System;
using Models;

namespace Services
{
    public interface IQuoteService
    {
        public int NrOfQuotes(string filter = null);

        public List<csFamousQuote> ReadQuotes(int? pageNumber = null, int? pageSize = null, string filter = null);
        public csFamousQuote ReadQuote(Guid id);

        public csFamousQuote UpdateQuote(csFamousQuote _src);

        public csFamousQuote CreateQuote(csFamousQuote _src);

        public csFamousQuote DeleteQuote(Guid id);
    }
}
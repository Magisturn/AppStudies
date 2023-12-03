using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Models;

namespace AppStudies.Pages.Studies
{
    public class csSentence
    {
        public Guid id { get; set; }
        public string Sentence { get; set; }
    }

	public class ModalsPostModel : PageModel
    {
        [FromQuery(Name = "idGet")]
        public string IdGet { get; set; }

        [BindProperty]
        public csSentence Latin { get; set; } = new csSentence();

        public string Message { get; set; }
        public IActionResult OnGet()
        {
            var rnd = new csSeedGenerator();
            Latin = new csSentence { id = Guid.NewGuid(), Sentence = rnd.LatinSentence };

            Message = $"Latin created: {Latin.id}, {Latin.Sentence}";

            return Page();
        }

        public IActionResult OnPost(Guid id)
        {
            Message = $"Post() fired with id: {id}";

            //Page is rendered as the postback is part of the form tag
            return Page();
        }

        public IActionResult OnPostSave(Guid id)
        {
            Message = $"PostSave() fired with id: {id}";

            //Page is rendered as the postback is part of the form tag
            return Page();
        }

        public IActionResult OnPostSaveLatin([FromBody] csModalData modalData)
        {
            Message = $"PostSaveLatin() fired with id: {Guid.Parse(modalData.postdata)}";

            //Page is not rerendered as the Post is triggered from a button click outside the form
            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AppStudies.Pages
{
    public class csModalData
    {
        public string postdata { get; set; } 
    }

    public class ModalsLaunchModel : PageModel
    {
        public List<string> Messages { get; set; } = new List<string>();

        //As the modals does not send the data as a Form the [FromBody] needs to be used
        //try with curl command
        //curl --location 'https://localhost:11774/Studies/ModalsLaunch' --header 'Content-Type: application/json' --data '{"id":"123-123"}'
        //Page not reloaded
        public PartialViewResult OnPostDelete([FromBody] csModalData modalData)
        {
            Messages.Add($"OnPostDelete from js fired: Modal Guid: {modalData.postdata}");
            Messages.Add($"OnPostDelete Guid: {Guid.NewGuid()}");

            //Page not reloaded as Post is outside a form via javascript
            return Partial("Studies/Modals/_PartialModalsLaunch", Messages);
        }


        public IActionResult OnPostSelect(Guid groupId)
        {
            Messages.Add($"OnPostSelect fired: {groupId}");
            return Page();
        }
    }
}

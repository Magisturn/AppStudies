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
        public string Message { get; set; }

        //As the modals does not send the data as a Form the [FromBody] needs to be used
        //try with curl command
        //curl --location 'https://localhost:11774/Studies/ModalsLaunch' --header 'Content-Type: application/json' --data '{"id":"123-123"}'
        //Page not reloaded
        public IActionResult OnPostDelete([FromBody] csModalData modalData)
        {
            Message = $"OnPostDelete from Javascript fired: {modalData.postdata}";

            //Page not reloaded as Post is outside a form via javascript
            return Partial("Studies/_PartialModalsLaunch", Message);
        }


        public IActionResult OnPostSelect(Guid groupId)
        {
            Message = $"OnPostSelect fired: {groupId}";
            //return Partial("Studies/Modals/_PartialModalsLaunch", Message);
            return Page();
        }
    }
}

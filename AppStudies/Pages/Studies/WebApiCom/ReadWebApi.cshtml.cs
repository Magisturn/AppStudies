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
using Azure.Core;
using Newtonsoft.Json;

namespace AppStudies.Pages
{
    //Demonstrate how to use the model to present a list of objects
    public class ReadWebApiModel : PageModel
    {
        //Just like for WebApi
        HttpClient _httpClient;
        ILogger<ReadWebApiModel> _logger = null;

        //public member becomes part of the Model in the Razor page
        public List<csAlbum> Albums { get; set; } = new List<csAlbum>();


        //Will execute on a Get request
        public async Task<IActionResult> OnGet()
        {
            string uri = $"csalbums/read?flat=false";

            //Send the HTTP Message and await the repsonse
            HttpResponseMessage response = await _httpClient.GetAsync(uri);

            //Throw an exception if the response is not successful
            response.EnsureSuccessStatusCode();

            //Get the resonse data
            string s = await response.Content.ReadAsStringAsync();
            Albums = JsonConvert.DeserializeObject<List<csAlbum>>(s);
            return Page();
        }

        //Inject services just like in WebApi
        public ReadWebApiModel(IHttpClientFactory httpClientFactory, ILogger<ReadWebApiModel> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(name: "MusicWebApi");
        }
    }
}

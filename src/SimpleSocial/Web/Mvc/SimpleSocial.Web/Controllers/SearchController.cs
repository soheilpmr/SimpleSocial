﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSocial.Services.DataServices.SearchDataServices;

namespace SimpleSocial.Web.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ISearchServices searchServices;

        public SearchController(ISearchServices searchServices)
        {
            this.searchServices = searchServices;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index(string searchText)
        {
            var viewModel = this.searchServices.GetResultOfSearch(searchText,User);

            return View(viewModel);
        }
    }
}
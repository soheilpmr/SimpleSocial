﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleSocial.Services.Models.Account;
using SimpleSocial.Data.Common.Constants;
using SimpleSocial.Data.Models;
using SimpleSocial.Services.DataServices.FollowersDataServices;
using SimpleSocial.Services.DataServices.PostsServices;
using SimpleSocial.Services.DataServices.UsersDataServices;
using System.Threading.Tasks;

namespace SimpleSocial.Web.Controllers
{
    public class ProfilesController : BaseController
    {
        private readonly UserManager<SimpleSocialUser> userManager;
        private readonly IUserServices userServices;
        private readonly IPostServices postServices;
        private readonly IFollowersServices followersServices;

        public ProfilesController(
            UserManager<SimpleSocialUser> userManager,
            IUserServices userServices,
            IPostServices postServices,
            IFollowersServices followersServices)
        {
            this.userManager = userManager;
            this.userServices = userServices;
            this.postServices = postServices;
            this.followersServices = followersServices;
        }

        [Authorize]
        public async Task<IActionResult> Index(int userId)
        {
            if (this.userServices.GetUserId(User) == userId)
            {
                return RedirectToAction("MyProfile", "Account");
            }

            var currentUserId = userServices.GetUserId(User);

            var viewModel = new PostsFeedAndUserInfoViewModel()
            {
                CurrentUserInfo = await userServices.GetUserInfo(currentUserId, currentUserId),
                Posts = postServices.GetUserPosts(userId, currentUserId,0),
                UserProfileInfo = await userServices.GetUserInfo(userId, currentUserId),
            };

            if (viewModel.UserProfileInfo == null || viewModel.CurrentUserInfo == null)
            {
                var result = this.View("Error", this.ModelState);
                ViewData["Message"] = ErrorConstants.PageNotAvaivableMessage;
                result.StatusCode = (int)HttpStatusCode.NotFound;
                return result;
            }

            return this.View(viewModel);
        }

        public async Task<IActionResult> GetUserPosts(int pageNumber, int userId)
        {
            var currentUserId = userServices.GetUserId(User);
            var posts = postServices.GetUserPosts(userId, currentUserId, pageNumber);
            var viewModel = new PostsFeedAndUserInfoViewModel()
            {
                Posts = posts,
                CurrentUserInfo = await userServices.GetUserInfo(currentUserId, currentUserId),
                UserProfileInfo = await userServices.GetUserInfo(userId, currentUserId),
            };
            var partial = this.PartialView("Components/ListOfPosts/Default", viewModel);
            return partial;
        }

        [Authorize]
        public IActionResult SuccessfulAction(string message)
        {
            ViewData["Message"] = message;
            return View();
        }
    }
}
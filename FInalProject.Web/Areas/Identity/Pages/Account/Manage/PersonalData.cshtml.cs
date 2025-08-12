// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FInalProject.Application.Services;

namespace FInalProject.Web.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly IUserService _userService;

        public PersonalDataModel(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userService.GetPersonalDataAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }
            return Page();
        }
    }
}

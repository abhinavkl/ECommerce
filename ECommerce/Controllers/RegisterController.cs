﻿using DealManager.Models;
using ECommerce.Data.Account;
using ECommerce.Data.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ECommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public RegisterController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] object obj)
        {
            ResponseMessage message = new ResponseMessage();
            try
            {
                var register = JsonConvert.DeserializeObject<Register>(obj.ToString());

                if (!ModelState.IsValid)
                {
                    message.Message = string.Join("; ", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                    message.StatusCode = ResponseStatus.ERROR;
                    return new JsonResult(message);
                }
                else
                {
                    var user = new User()
                    {
                        UserName = register.Email,
                        Email = register.Email,
                        EmailConfirmed = true,
                        FirstName= register.FirstName,
                        LastName= register.LastName
                    };

                    var result = await userManager.CreateAsync(user, register.Password);

                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim("FirstName", register.FirstName),
                            new Claim("LastName", register.LastName),
                            new Claim("FullName", register.FirstName + " " + register.LastName)
                        };

                        await userManager.AddClaimsAsync(user, claims);

                        message.Message = "Registration successful."+Environment.NewLine+"You will be redirected to Login Page.";
                        message.StatusCode = ResponseStatus.SUCCESS;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            message.Message += error.Description + Environment.NewLine;
                        }
                        message.StatusCode= ResponseStatus.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.StatusCode = ResponseStatus.EXCEPTION;
            }
            return new JsonResult(message);
        }

    }
}

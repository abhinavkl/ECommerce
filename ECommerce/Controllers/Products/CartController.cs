﻿using ECommerce.Data;
using ECommerce.Data.Account;
using ECommerce.Models;
using ECommerce.Models.Ecommerce;
using ECommerce.Models.EcommerceExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ECommerce.Controllers.Products
{
    [Route("[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly EcommerceContext ecommerceContext;
        private readonly ILogger<CartController> _logger;
        private readonly IConfiguration configuration;
        private readonly IOptions<ProductFilters> filters;

        public CartController(EcommerceContext ecommerceContext,
            ILogger<CartController> logger,
            IConfiguration configuration,
            IOptions<ProductFilters> filters)
        {
            this.ecommerceContext = ecommerceContext;
            _logger = logger;
            this.configuration = configuration;
            this.filters = filters;
        }


        [HttpPost]
        [Route("[action]")]
        [ActionName("Add")]
        public JsonResult Add([FromBody] object obj)
        {
            var message = new ResponseMessage();

            try
            {
                var currentUserId = this.User.Identity.GetUserId();
                var cartItem = JsonConvert.DeserializeObject<Cart>(obj.ToString());
                var dbCartItem = ecommerceContext.Carts
                                .Where(i => i.ProductId == cartItem.ProductId && i.UserId == currentUserId);

                if (dbCartItem.Any())
                {
                    message.Message = "Product already added to cart";
                    message.StatusCode = ResponseStatus.ERROR;
                    return new JsonResult(message);
                }
                else if (cartItem.ProductId == 0 || cartItem.Quantity == 0)
                {
                    message.Message = "Can't add to cart";
                    message.StatusCode = ResponseStatus.ERROR;
                    return new JsonResult(message);
                }
                else
                {
                    cartItem.UserId = currentUserId;
                    cartItem.UpdatedOn = DateTime.Now;
                    ecommerceContext.Carts.Add(cartItem);
                    ecommerceContext.SaveChanges();

                    var product = ecommerceContext.Products
                         .Include(i => i.Brand)
                         .Include(i => i.Category)
                         .Include(i => i.IndividualCategory)
                         .Include(i => i.Favorites)
                         .Where(i => i.ProductId == cartItem.ProductId);

                    message.Data = product.First().GetProductDto(this.User);
                    message.Message = "Product added to cart.";
                    message.StatusCode = ResponseStatus.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.StatusCode = ResponseStatus.ERROR;
            }

            return new JsonResult(message);
        }

        [HttpGet]
        public JsonResult Get(int pageNumber=1)
        {
            var message = new ResponseMessage();
            try
            {
                var currentUserId = this.User.Identity.GetUserId();
                var products=ecommerceContext.Carts
                                    .Include(i => i.Product).DefaultIfEmpty()
                                    .Include(i => i.Product).ThenInclude(i => i.Brand).DefaultIfEmpty()
                                    .Include(i => i.Product).ThenInclude(i => i.Category).DefaultIfEmpty()
                                    .Include(i => i.Product).ThenInclude(i => i.IndividualCategory).DefaultIfEmpty()
                                    .Include(i => i.Product).ThenInclude(i => i.ProductQuantities).ThenInclude(i => i.Size)
                                    .Where(i => i.UserId == currentUserId)
                                    .OrderByDescending(i => i.UpdatedOn)
                                    .Select(i => i.Product).GetProductDtos(this.User);

                var productCount = this.filters.Value.ProductCount;
                var totalRecords = products.Count();
                var totalPages = (totalRecords + productCount) / productCount;

                message.Data = new
                {
                    Result = products.PaginateData(pageNumber, productCount),
                    TotalPages = totalPages,
                    PageNumber = pageNumber
                };
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.StatusCode = ResponseStatus.ERROR;
            }
            return new JsonResult(message);
        }


        [HttpPost]
        [Route("[action]")]
        [ActionName("Remove")]
        public JsonResult Remove([FromBody] object obj)
        {
            var message = new ResponseMessage();
            try
            {
                var currentUserId = this.User.Identity.GetUserId();
                var cartItem = JsonConvert.DeserializeObject<Cart>(obj.ToString());
                var dbCartItem = ecommerceContext.Carts
                                .Where(i => i.ProductId == cartItem.ProductId && i.UserId == currentUserId);

                if (!dbCartItem.Any())
                {
                    message.Message = "Product not found in cart";
                    message.StatusCode = ResponseStatus.ERROR;
                    return new JsonResult(message);
                }
                else if (cartItem.ProductId == 0 || cartItem.Quantity == 0)
                {
                    message.Message = "Can't remove from cart";
                    message.StatusCode = ResponseStatus.ERROR;
                    return new JsonResult(message);
                }
                else
                {
                    ecommerceContext.Carts.Remove(dbCartItem.First());
                    ecommerceContext.SaveChanges();
                    message.Message = "Product removed from cart.";
                    message.StatusCode = ResponseStatus.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.StatusCode = ResponseStatus.ERROR;
            }

            return new JsonResult(message);
        }
    }
}
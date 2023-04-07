﻿using ECommerce.Models;
using ECommerce.Data;
using ECommerce.Data.Authentication;
using ECommerce.Data.Products;
using ECommerce.Models.Ecommerce;
using ECommerce.Models.EcommerceExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ECommerce.Controllers.Products
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly EcommerceContext ecommerceContext;
        private readonly ILogger<ProductsController> _logger;
        private readonly IConfiguration configuration;
        private readonly IOptions<ProductFilters> filters;

        public ProductsController(EcommerceContext ecommerceContext, 
            ILogger<ProductsController> logger,
            IConfiguration configuration,
            IOptions<ProductFilters> filters)
        {
            this.ecommerceContext = ecommerceContext;
            _logger = logger;
            this.configuration = configuration;
            this.filters = filters;
        }

        [HttpGet]
        public IActionResult Get(
            int? productCount,
            string sortBy,
            string sortOrder, 
            string brands=null,
            string categories=null,
            string individualCategories=null)
        {
            var message = new ResponseMessage();
            try
            {
                productCount = productCount ?? this.filters.Value.ProductCount;
                sortBy = sortBy ?? this.filters.Value.SortBy;
                sortOrder = sortOrder ?? this.filters.Value.SortOrder;
                
                var filters = new ProductFilters()
                {
                    ProductCount=productCount.Value,
                    SortBy=sortBy,
                    SortOrder=sortOrder,
                    BrandIds= Utilities.GetArray(brands,","),
                    CategoryIds=Utilities.GetArray(categories,","),
                    IndividualCategoryIds=Utilities.GetArray(individualCategories,",")
                };

                var products = ecommerceContext.Products
                                .Include(i => i.Brand).DefaultIfEmpty()
                                .Include(i => i.Category).DefaultIfEmpty()
                                .Include(i => i.IndividualCategory).DefaultIfEmpty();

                message.Data= products.GetProductDtos(filters);
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

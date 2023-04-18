﻿using ECommerce.Data;
using ECommerce.Data.Account;
using ECommerce.Data.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using System.Security.Claims;

namespace ECommerce.Models.Ecommerce
{
    public partial class Order
    {
    }

    public static class OrderExt
    {
        public static async Task<List<OrderItem>> GetOrderDtos(
            this IQueryable<Order> orders,
            UserManager<User> userManager,
            ClaimsPrincipal principal,
            string dateFilter)
        {

            switch (dateFilter)
            {
                case "365":
                    orders = orders.Where(i => EF.Functions.DateDiffDay(i.PlacedOn.Date, DateTime.Today) <= 365);
                    break;
                case "180":
                    orders = orders.Where(i => EF.Functions.DateDiffDay(i.PlacedOn.Date, DateTime.Today) <= 180);
                    break;
                case "90":
                    orders = orders.Where(i => EF.Functions.DateDiffDay(i.PlacedOn.Date, DateTime.Today) <= 90);
                    break;
                case "30":
                default:
                    orders = orders.Where(i => EF.Functions.DateDiffDay(i.PlacedOn.Date, DateTime.Today) <= 30);
                    break;
            }

            var allOrderedDates = orders.Select(i => i.PlacedOn.Date).Distinct().ToList();

            var orderItems = new List<OrderItem>();

            foreach (var date in allOrderedDates.OrderByDescending(i=>i))
            {
                var currentDateOrders = orders.Where(i => i.PlacedOn.Date == date).OrderByDescending(i=>i.PlacedOn).ToList();

                var orderItem = new OrderItem() 
                { 
                    Date=date.ToLongDateString(),
                    OrderInstances=new List<OrderInstanceItem>()
                };

                var instances=currentDateOrders.Select(i=>i.OrderInstanceId).Distinct().ToList();

                var orderInstances = new List<OrderInstanceItem>();
                foreach (var instance in instances)
                {
                    var currentInstances = currentDateOrders.Where(i => i.OrderInstanceId == instance);
                    var currentInstanceFirstOrderItem = currentInstances.FirstOrDefault();

                    var user = await userManager.FindByIdAsync(currentInstanceFirstOrderItem.UserId.ToString());
                    var claims = await userManager.GetClaimsAsync(user);

                    var orderInstance = new OrderInstanceItem()
                    {
                        InstanceId=instance,
                        UserId= currentInstanceFirstOrderItem.UserId,
                        UserName=Utilities.GetFullName(claims),
                        Products=new List<ProductDto>()
                    };

                    foreach(var order in currentInstances)
                    {
                        var productDto = order.Product.GetProductDto(principal);
                        productDto.Quantity=order.Quantity;
                        orderInstance.Products.Add(productDto);
                    }

                    orderInstances.Add(orderInstance);
                }
                
                orderItem.OrderInstances = orderInstances;
                orderItems.Add(orderItem);
            }

            return orderItems;
        }

        public static OrderDto GetOrderDto(this Order order)
        {
            return new OrderDto
            { 
                OrderId= order.OrderId,
                PlacedOn= order.PlacedOn,
                ProductId= order.ProductId,
                Quantity= order.Quantity,
                UserId= order.UserId
            };
        }

    }

}
﻿using System;
using System.Collections.Generic;

namespace ECommerce.Models.Ecommerce
{
    public partial class Favourite
    {
        public int FavouriteId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
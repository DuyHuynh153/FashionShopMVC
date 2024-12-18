﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionShopMVC.Models.Domain
{
    [Table("Users")]
    public class User : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<FavoriteProduct> FavoriteProducts { get; set; }
    }
}

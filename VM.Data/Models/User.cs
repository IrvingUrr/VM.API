﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace VM.Data.Models
{
    public partial class User
    {
        public User()
        {
            UserPurchases = new HashSet<UserPurchase>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserPurchase> UserPurchases { get; set; }
    }
}
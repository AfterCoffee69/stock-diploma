﻿namespace Stock.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public List<Product> Products { get; set; }
    }
}

using CarProduct.Persistence.Enums;
using CarProduct.Persistence.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace CarProduct.Persistence.Models
{
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string VehicleId { get; set; }
        public int Order { get; set; }

        public StockTypes StockType { get; set; }
        public FuelTypes FuelType { get; set; }
        public string StockNumber { get; set; }
        public string VIN { get; set; }
        public string Transmission { get; set; }
        public string Title { get; set; }
        public double Mileage { get; set; }
        public decimal Price { get; set; }
        public DateTime? OnActualized { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.NotProcessed;

        public int ProductsPageId { get; set; }
        public ProductsPage ProductsPage { get; set; }

        public ICollection<string> ProductReviews { get; set; } = new HashSet<string>();
        public ICollection<string> ImageLinks { get; set; } = new HashSet<string>();
        public ICollection<string> Badges { get; set; } = new HashSet<string>();
        public ICollection<string> Features { get; set; } = new HashSet<string>();
        
    }
}

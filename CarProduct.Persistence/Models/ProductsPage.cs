using System;
using CarProduct.Persistence.Models.Interfaces;
using System.Collections.Generic;
using CarProduct.Persistence.Enums;

namespace CarProduct.Persistence.Models
{
    public class ProductsPage : IEntity
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public int Order { get; set; }
        public DateTime? OnActualized { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.NotProcessed;

        public int ScreenshotId { get; set; }
        public Attachment Screenshot { get; set; }
        
        public int ScrapeRequestId { get; set; }
        public ScrapeRequest ScrapeRequest { get; set; }

        public IEnumerable<string> ProductVehicleIds { get; set; } = new HashSet<string>();
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}

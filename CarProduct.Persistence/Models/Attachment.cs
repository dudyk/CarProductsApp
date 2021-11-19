using CarProduct.Persistence.Models.Interfaces;

namespace CarProduct.Persistence.Models
{
    public class Attachment : IEntity
    {
		public int Id { get; set; }
        public string FileName { get; set; }
        public string GenFileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
	}
}
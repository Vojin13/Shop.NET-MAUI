namespace Android_Ispit.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public CategoryDTO? Category { get; set; }
        public List<string> Images { get; set; } = new();

        public string MainImage => Images.Count > 0 ? Images[0] : "https://picsum.photos/seed/" + Id + "/400/300";
        public string PriceDisplay => $"${Price:0.##}";
        public string CategoryName => Category?.Name ?? "";
    }
}

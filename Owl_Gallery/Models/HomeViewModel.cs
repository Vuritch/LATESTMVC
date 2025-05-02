using Owl_Gallery.Models;
namespace Owl_Gallery.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> TrendingProducts { get; set; }
        public List<Product> BestSellers { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> Products { get; set; }

    }
}
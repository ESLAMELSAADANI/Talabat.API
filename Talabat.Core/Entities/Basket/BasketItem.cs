namespace Talabat.Core.Entities.Basket
{
    public class BasketItem
    {
        //Item is => Product
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }//Price of product when added in basket
                                          //price of product is constant in system
                                          //we make discount on basketItem Price not the actual product
                                          //like if say when customer order this product with quantity > 10 , make discount on the actual price
                                          //so price will be => (productPrice * 0.10)
        public int Quantity { get; set; }
        public string Category { get; set; } = null!;
        public string Brand { get; set; } = null!;
    }
}
using Microsoft.eShopWeb.ApplicationCore.Services;

namespace eShopWeb.ApplicationCore.Services
{
    public class BasketCalculator
    {
        public int CalculateQuantity(int currentQuantity, int newQuantity)
        {
            // Implement "Buy 2 Get 1 Free" Black Friday sale
            if (newQuantity == 2)
            {
                newQuantity = 3;
            }

            if (currentQuantity > 2 && newQuantity < 2)
            {
                --newQuantity;
            }
            GuardAssertions.OutOfRange(newQuantity, nameof(newQuantity), 0, int.MaxValue);

            return newQuantity;
        }
    }
}
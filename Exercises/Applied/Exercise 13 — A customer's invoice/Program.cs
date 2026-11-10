Console.WriteLine("Enter your product name: ");
string productName = Console.ReadLine();
Console.WriteLine("Enter the product price: ");
float productPrice = Convert.ToSingle(Console.ReadLine());
Console.WriteLine("Enter the product quantity: ");
int productQuantity = Convert.ToInt32(Console.ReadLine());
float totalPrice = productPrice * productQuantity;
if (totalPrice < 150)   
{
    Console.WriteLine($"The total price for {productQuantity} units of {productName} is ${totalPrice}. You are not eligible for a discount.");
}
else if (totalPrice < 400 && totalPrice >= 150)
{
    Console.WriteLine($"The total price for {productQuantity} units of {productName} is ${totalPrice - (totalPrice * 0.04)}. You are eligible for a discount 4%.");
}
else
{
    Console.WriteLine($"The total price for {productQuantity} units of {productName} is ${totalPrice - (totalPrice * 0.08)}. You are eligible for a discount 8%.");
}

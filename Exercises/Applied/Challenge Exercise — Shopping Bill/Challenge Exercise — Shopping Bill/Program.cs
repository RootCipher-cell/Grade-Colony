Console.WriteLine("first product");
Console.ReadLine();
Console.WriteLine("1st Unit price: $");
float firstUnitPrice = float.Parse(Console.ReadLine());
Console.WriteLine("1st Quantity: ");
int firstQuantity = int.Parse(Console.ReadLine());

Console.WriteLine("2nd second product");
Console.ReadLine();
Console.WriteLine("2nd Unit price: $");
float secondUnitPrice = float.Parse(Console.ReadLine());
Console.WriteLine("2nd Quantity: ");
int secondQuantity = int.Parse(Console.ReadLine());

Console.WriteLine("3rd product");
Console.ReadLine();
Console.WriteLine("3rd Unit price: $");
float thirdUnitPrice = float.Parse(Console.ReadLine());
Console.WriteLine("3rd Quantity: ");
int thirdQuantity = int.Parse(Console.ReadLine());

float shoppingbill = (firstUnitPrice * firstQuantity) + (secondUnitPrice * secondQuantity) + (thirdUnitPrice * thirdQuantity);

//5% discount if total bill is greater than $200
if (shoppingbill > 200)
{
    shoppingbill = shoppingbill - (shoppingbill * 0.05f);
}
//10% discount if total bill is greater than $500
if (shoppingbill > 500)
{
    shoppingbill = shoppingbill - (shoppingbill * 0.10f);
}
//display total without discount
Console.WriteLine("Total bill without discount: $" + ((firstUnitPrice * firstQuantity) + (secondUnitPrice * secondQuantity) + (thirdUnitPrice * thirdQuantity)));
//display only discount amount
float discountAmount = ((firstUnitPrice * firstQuantity) + (secondUnitPrice * secondQuantity) + (thirdUnitPrice * thirdQuantity)) - shoppingbill;
Console.WriteLine("Discount amount: $" + discountAmount);
//display final total after discount 
 Console.WriteLine("Final bill: $" + shoppingbill);
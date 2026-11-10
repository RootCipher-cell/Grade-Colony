Console.WriteLine("Enter the first number:");
int num1 = Convert.ToInt32(Console.ReadLine());
Console.WriteLine("Enter the second number:");
int num2 = Convert.ToInt32(Console.ReadLine());
if (num1 > num2)
{
    Console.WriteLine("The first number is larger.");
}
else if (num2 > num1)
{
    Console.WriteLine("The second number is larger.");
}
else
{
    Console.WriteLine("Both numbers are equal.");
}
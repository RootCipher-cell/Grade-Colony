float firstnumber, secondnumber, sum, difference, product, quotient;


Console.WriteLine("Enter the first number:");
float firstNumber = float.Parse(Console.ReadLine());
Console.WriteLine("Enter the second number:");
float secondNumber = float.Parse(Console.ReadLine());

sum = firstNumber + secondNumber;
Console.WriteLine("Sum is :" + sum);
difference = firstNumber - secondNumber;
Console.WriteLine("Difference is :" + difference);
product = firstNumber * secondNumber;
Console.WriteLine("Product is :" + product);
quotient = firstNumber / secondNumber;
Console.WriteLine("Quotient is :" + quotient);  
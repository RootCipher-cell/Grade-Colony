using System.ComponentModel.Design;

Console.WriteLine("Enter first number");
double num1= double.Parse(Console.ReadLine());
Console.WriteLine("Enter second number");
double num2 = double.Parse(Console.ReadLine());
Console.WriteLine("Enter an operator (+, -, *, /):");
string op = Console.ReadLine();
if (op == "+") 
{
    Console.WriteLine(num1 + num2); 
}
else if (op == "-") 
{ 
    Console.WriteLine(num1 - num2); 
}
else if (op == "*") 
{ 
    Console.WriteLine(num1 * num2); 
}
else if (op == "/") 
{
    Console.WriteLine(num1 / num2); 
}
else if (op!= "+" && op != "-" && op != "*" && op != "/")
{
    Console.WriteLine("Invalid operator");
}
else if (op == "/" && num2 == 0)
{
    Console.WriteLine("Cannot divide by zero");
}
// very nice 
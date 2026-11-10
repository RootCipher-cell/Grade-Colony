Console.WriteLine("Enter your grade");
float grade = float.Parse(Console.ReadLine());
if (grade > 20 || grade < 0)
{
    Console.WriteLine("Invalid Grade");
}
else if (grade < 10)
{
    Console.WriteLine("Fail");
}
else if (grade >= 10 && grade < 11.99)
{
    Console.WriteLine("Pass");
}
else if (grade >= 12 && grade < 13.99)
{
    Console.WriteLine("Good");
}
else if (grade >= 14 && grade < 15.99)
{
    Console.WriteLine("Very Good");
}
else if (grade >= 16 && grade <= 20)
{
    Console.WriteLine("Excellent");
}
else
{
    Console.WriteLine("Invalid grade entered.");
}

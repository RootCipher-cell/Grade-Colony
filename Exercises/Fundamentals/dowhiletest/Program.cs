
int n;

do
{
    Console.WriteLine("enter your number between 10 and 15");
    n = int.Parse(Console.ReadLine());
} while (n < 10 || n > 15);

int s = 0; 
for (int i = 1; i <= n; i++)
{
    s += i;
}

Console.WriteLine("The sum of numbers is  " + s + " and the number is " + n);
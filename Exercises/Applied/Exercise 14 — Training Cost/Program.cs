int dailyRate = 0;

Console.WriteLine("Are you a student or professional?");
string userType = Console.ReadLine()?.Trim() ?? "";

Console.WriteLine("Please enter your total days with us:");
if (!int.TryParse(Console.ReadLine(), out int totalDays) || totalDays <= 0)
{
    Console.WriteLine("Invalid number of days.");
    return;
}

if (userType.Equals("Student", StringComparison.OrdinalIgnoreCase))
{
    dailyRate = 35;
}
else if (userType.Equals("Professional", StringComparison.OrdinalIgnoreCase))
{
    dailyRate = 60;
}
else
{
    Console.WriteLine("Invalid user type. Enter Student or Professional.");
    return;
}

int grossAmount = dailyRate * totalDays;
double discount = 0;

if (grossAmount > 250)
{
    discount = grossAmount * 0.10;
}

double finalAmount = grossAmount - discount;

Console.WriteLine($"Gross Amount: {grossAmount}");
Console.WriteLine($"Discount: {discount}");
Console.WriteLine($"Final Amount: {finalAmount}");
int sum = 0;
int max = 0;
int count = 0;

for (int i = 1; i <= 8; i++)
{
    Console.WriteLine("Enter grade " + i + " (0-20):");
    int grade = int.Parse(Console.ReadLine());

    while (grade < 0 || grade > 20)
    {
        Console.WriteLine("Invalid grade. Enter again (0-20):");
        grade = int.Parse(Console.ReadLine());
    }

    sum = sum + grade;

    if (grade > max)
    {
        max = grade;
    }

    if (grade >= 10)
    {
        count = count + 1;
    }
}

double average = sum / 8.0;

Console.WriteLine("Sum: " + sum);
Console.WriteLine("Average: " + average);
Console.WriteLine("Maximum: " + max);
Console.WriteLine("Students with at least 10/20: " + count);
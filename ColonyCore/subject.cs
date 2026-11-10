public class Subject
{
    public string Name { get; set; }
    public double Grade { get; set; }
    public double Coefficient { get; set; }

    public Subject(string name, double grade, double coefficient)
    {
        Name = name;
        Grade = grade;
        Coefficient = coefficient;
    }

    // The label for this subject's current grade
    public string Mention => GetMention(Grade);

    // Same labels as your original code, but with no gaps between ranges
    public static string GetMention(double grade)
    {
        if (grade < 0 || grade > 20) return "Invalid grade";
        if (grade < 10) return "Fail";
        if (grade < 12) return "Pass";
        if (grade < 14) return "Good";
        if (grade < 16) return "Very Good";
        return "Excellent";
    }
}
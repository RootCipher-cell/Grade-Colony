using System.Globalization;

public class GradeTracker
{
    private readonly List<Subject> _subjects = new List<Subject>();

    public IReadOnlyList<Subject> Subjects => _subjects;

    public void Add(Subject subject)
    {
        _subjects.Add(subject);
    }

    public bool Remove(int index)
    {
        if (index < 0 || index >= _subjects.Count) return false;
        _subjects.RemoveAt(index);
        return true;
    }

    // Weighted average: sum(grade * coefficient) / sum(coefficients)
    public double CalculateAverage()
    {
        double total = 0;
        double totalCoefficients = 0;

        foreach (Subject s in _subjects)
        {
            total += s.Grade * s.Coefficient;
            totalCoefficients += s.Coefficient;
        }

        return totalCoefficients == 0 ? 0 : total / totalCoefficients;
    }

    // One subject per line: name;grade;coefficient
    public void Save(string path)
    {
        var lines = new List<string>();

        foreach (Subject s in _subjects)
        {
            lines.Add(string.Join(";",
                s.Name,
                s.Grade.ToString(CultureInfo.InvariantCulture),
                s.Coefficient.ToString(CultureInfo.InvariantCulture)));
        }

        File.WriteAllLines(path, lines);
    }

    public void Load(string path)
    {
        if (!File.Exists(path)) return;

        _subjects.Clear();

        foreach (string line in File.ReadAllLines(path))
        {
            string[] parts = line.Split(';');
            if (parts.Length < 3) continue;

            bool gradeOk = double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double grade);
            bool coefOk = double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double coef);

            if (gradeOk && coefOk)
            {
                _subjects.Add(new Subject(parts[0], grade, coef));
            }
        }
    }
}
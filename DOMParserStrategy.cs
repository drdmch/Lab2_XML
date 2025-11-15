using System.Collections.Generic;
using System.Xml;

namespace lab2XML;

public class DOMParserStrategy : IParserStrategy
{
    public List<MainPageViewModel.StudentItem> ParseAndSearch(MainPageViewModel.StudentItem searchCriteria, string xmlFilePath)
    {
        List<MainPageViewModel.StudentItem> students = new List<MainPageViewModel.StudentItem>();
        XmlDocument document = new XmlDocument();
        document.Load(xmlFilePath);

        XmlNodeList nodes = document.GetElementsByTagName("student");
        foreach (XmlNode node in nodes)
        {
            var student = ParseStudentNode(node);

            if (StudentMatchesCriteria(student, searchCriteria))
            {
                students.Add(student);
            }
        }

        return students;
    }

    private MainPageViewModel.StudentItem ParseStudentNode(XmlNode node)
    {
        string name = node["name"]?.InnerText ?? "";
        string faculty = node["faculty"]?.InnerText ?? "";
        string department = node["department"]?.InnerText ?? "";
        var disciplinesNode = node["courses"];
        string disciplinesText = "";
        double totalGrades = 0;
        int gradeCount = 0;

        if (disciplinesNode != null)
        {
            foreach (XmlNode discNode in disciplinesNode.ChildNodes)
            {
                string title = discNode["course_name"]?.InnerText ?? "";
                string gradeStr = discNode["grade"]?.InnerText ?? "0";
                double grade = double.Parse(gradeStr);
                disciplinesText += $"{title}: {grade}\n";
                totalGrades += grade;
                gradeCount++;
            }
        }

        double avgGrade = gradeCount > 0 ? totalGrades / gradeCount : 0;

        return new MainPageViewModel.StudentItem
        {
            Name = name,
            Faculty = faculty,
            Department = department,
            Disceplines = disciplinesText.Trim(),
            AVGGrade = avgGrade
        };
    }

    private bool StudentMatchesCriteria(MainPageViewModel.StudentItem student, MainPageViewModel.StudentItem criteria)
    {
        return student.Name.Contains(criteria.Name)
            && student.Faculty.Contains(criteria.Faculty)
            && student.Department.Contains(criteria.Department)
            && student.AVGGrade >= criteria.AVGGrade;
    }
}

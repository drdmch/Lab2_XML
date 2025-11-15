using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace lab2XML;

public class LinqToXmlParserStrategy : IParserStrategy
{
    public List<MainPageViewModel.StudentItem> ParseAndSearch(MainPageViewModel.StudentItem searchCriteria, string xmlFilePath)
    {
        XDocument xdoc = XDocument.Load(xmlFilePath);

        var students = xdoc.Descendants("student")
            .Select(s =>        new MainPageViewModel.StudentItem
            {
                Name = (string)s.Element("name"),
                Faculty = (string)s.Element("faculty"),
                Department = (string)s.Element("department"),
                Disceplines = string.Join("\n", s.Element("courses")?.Elements("course")
                    .Select(c => $"{(string)c.Element("course_name")}: {(string)c.Element("grade")}")) ?? "",
                AVGGrade = s.Element("courses")?.Elements("course")
                    .Average(c => (double)c.Element("grade")) ??0,
            })
            .Where(student =>
                student.Name.Contains(searchCriteria.Name)
                && student.Faculty.Contains(searchCriteria.Faculty)
                && student.Department.Contains(searchCriteria.Department)
                && student.AVGGrade >= searchCriteria.AVGGrade)
            .ToList();

        return students;
    }
}

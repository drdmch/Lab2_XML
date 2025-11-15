using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace lab2XML;

public class SAXParserStrategy : IParserStrategy
{
    public List<MainPageViewModel.StudentItem> ParseAndSearch(MainPageViewModel.StudentItem searchCriteria, string xmlFilePath)
    {
        List<MainPageViewModel.StudentItem> students = new List<MainPageViewModel.StudentItem>();
        using (XmlReader reader = XmlReader.Create(xmlFilePath))
        {
            MainPageViewModel.StudentItem currentStudent = new MainPageViewModel.StudentItem();
            List<MainPageViewModel.Discepline> disciplines = new List<MainPageViewModel.Discepline>();
            bool inStudent = false;
            bool inDisciplines = false;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "student":
                            inStudent = true;
                            currentStudent = new MainPageViewModel.StudentItem();
                            disciplines.Clear();
                            break;
                        case "name":
                            if (inStudent)
                                currentStudent.Name = reader.ReadElementContentAsString();
                            break;
                        case "faculty":
                            if (inStudent)
                                currentStudent.Faculty = reader.ReadElementContentAsString();
                            break;
                        case "department":
                            if (inStudent)
                                currentStudent.Department = reader.ReadElementContentAsString();
                            break;
                        case "courses":
                            inDisciplines = true;
                            break;
                        case "course":
                            if (inDisciplines)
                            {
                                var discipline = ReadDiscipline(reader);
                                disciplines.Add(discipline);
                            }
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "disciplines")
                    {
                        inDisciplines = false;
                    }
                    else if (reader.Name == "student")
                    {
                        inStudent = false;
                        double totalGrades = disciplines.Sum(d => double.Parse(d.Grade));
                        int gradeCount = disciplines.Count;
                        currentStudent.AVGGrade = gradeCount > 0 ? totalGrades / gradeCount : 0;
                        currentStudent.Disceplines = string.Join("\n", disciplines.Select(d => $"{d.Title}: {d.Grade}"));
                        if (StudentMatchesCriteria(currentStudent, searchCriteria))
                        {
                            students.Add(currentStudent);
                        }
                    }
                }
            }
        }
        return students;
    }

    private MainPageViewModel.Discepline ReadDiscipline(XmlReader reader)
    {
        string title = "";
        string grade = "0";
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "course_name":
                        title = reader.ReadElementContentAsString();
                        break;
                    case "grade":
                        grade = reader.ReadElementContentAsString();
                        break;
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "course")
            {
                break;
            }
        }
        return new MainPageViewModel.Discepline { Title = title, Grade = grade };
    }

    private bool StudentMatchesCriteria(MainPageViewModel.StudentItem student, MainPageViewModel.StudentItem criteria)
    {
        return student.Name.Contains(criteria.Name)
            && student.Faculty.Contains(criteria.Faculty)
            && student.Department.Contains(criteria.Department)
            && student.AVGGrade >= criteria.AVGGrade;
    }
}

using System.Collections.Generic;
using System.Xml;

namespace lab2XML;

public class XMLDataHandlers
{
    public string XmlFilePath { get; set; }
    public string XslFilePath { get; set; }

    public XmlDocument CreateXmlDocument(List<MainPageViewModel.StudentItem> students)
    {
        XmlDocument xmlDoc = new XmlDocument();

        XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(xmlDeclaration);

        XmlElement rootElement = xmlDoc.CreateElement("students");
        xmlDoc.AppendChild(rootElement);

        foreach (var student in students)
        {
            XmlElement studentElement = xmlDoc.CreateElement("student");

            XmlElement nameElement = xmlDoc.CreateElement("name");
            nameElement.InnerText = student.Name;
            studentElement.AppendChild(nameElement);

            XmlElement departmentElement = xmlDoc.CreateElement("department");
            departmentElement.InnerText = student.Department;
            studentElement.AppendChild(departmentElement);

            XmlElement facultyElement = xmlDoc.CreateElement("faculty");
            facultyElement.InnerText = student.Faculty;
            studentElement.AppendChild(facultyElement);

            XmlElement disciplinesElement = xmlDoc.CreateElement("disciplines");
            disciplinesElement.InnerText = student.Disceplines;
            studentElement.AppendChild(disciplinesElement);

            XmlElement avgGradeElement = xmlDoc.CreateElement("avgGrade");
            avgGradeElement.InnerText = student.AVGGrade.ToString();
            studentElement.AppendChild(avgGradeElement);

            rootElement.AppendChild(studentElement);
        }

        return xmlDoc;
    }
}

using System.Collections.Generic;

namespace lab2XML;

public interface IParserStrategy
{
    List<MainPageViewModel.StudentItem> ParseAndSearch(MainPageViewModel.StudentItem student, string xmlFilePath);
}

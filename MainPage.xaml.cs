using System;
using System.Xml;
using System.Xml.Xsl;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using System.IO;
using System.Threading.Tasks;

namespace lab2XML;

public partial class MainPage : ContentPage
{
    private MainPageViewModel mainPageViewModel;
    private XMLDataHandlers dataHandlers;
    private IParserStrategy parserStrategy;
    private List<MainPageViewModel.StudentItem> lastSearchResult;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = mainPageViewModel = new MainPageViewModel();
        dataHandlers = new XMLDataHandlers();
    }

    public async void Load_XML_File(object sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } },
                { DevicePlatform.macOS, new[] { "xml" } },
            });

            var pickOptions = new PickOptions
            {
                PickerTitle = "Please select an XML file",
                FileTypes = customFileType
            };

            var result = await FilePicker.PickAsync(pickOptions);

            if (result != null)
            {
                dataHandlers.XmlFilePath = result.FullPath;
                await DisplayAlert("Success", "XML file loaded successfully.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load XML file: {ex.Message}", "OK");
        }
    }

    public async void Load_XSL_File(object sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xsl" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xsl", ".xslt" } },
                { DevicePlatform.macOS, new[] { "xsl", "xslt" } },
            });

            var pickOptions = new PickOptions
            {
                PickerTitle = "Please select an XSL file",
                FileTypes = customFileType
            };

            var result = await FilePicker.PickAsync(pickOptions);

            if (result != null)
            {
                dataHandlers.XslFilePath = result.FullPath;
                await DisplayAlert("Success", "XSL file loaded successfully.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load XSL file: {ex.Message}", "OK");
        }
    }

    public void Search_Submit(object sender, EventArgs e)
    {
        if (Linq.IsChecked == true)
        {
            parserStrategy = new LinqToXmlParserStrategy();
        }
        else if (Dom.IsChecked == true)
        {
            parserStrategy = new DOMParserStrategy();
        }
        else if (Sax.IsChecked == true)
        {
            parserStrategy = new SAXParserStrategy();
        }
        else
        {
            parserStrategy = new LinqToXmlParserStrategy();
        }

        string xmlFilePath = dataHandlers.XmlFilePath;
        if (string.IsNullOrEmpty(xmlFilePath))
        {
            DisplayAlert("Error", "Please load an XML file first.", "OK");
            return;
        }

        var searchCriteria = new MainPageViewModel.StudentItem
        {
            Name = Name.Text ?? "",
            Faculty = Faculty.Text ?? "",
            Department = Department.Text ?? "",
            AVGGrade = double.TryParse(Grade.Text, out double grade) ? grade : 0,
            Disceplines = ""
        };

        lastSearchResult = parserStrategy.ParseAndSearch(searchCriteria, xmlFilePath);
        MyCollectionViews.ItemsSource = lastSearchResult;
    }

    public void Clear_Fields(object sender, EventArgs e)
    {
        Name.Text = "";
        Faculty.Text = "";
        Department.Text = "";
        Grade.Text = "";
        MyCollectionViews.ItemsSource = null;
    }

    public void Export_HTML(object sender, EventArgs e)
    {
        if (lastSearchResult == null || lastSearchResult.Count == 0)
        {
            DisplayAlert("Error", "No data to export. Please perform a search first.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(dataHandlers.XslFilePath))
        {
            DisplayAlert("Error", "Please load an XSL file first.", "OK");
            return;
        }

        string fileName = FileNameEntry.Text;
        if (string.IsNullOrWhiteSpace(fileName))
        {
            DisplayAlert("Error", "Please enter a valid file name.", "OK");
            return;
        }

        if (!fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".html";
        }

        string downloadsPath = GetDownloadsPath();
        string outputHtmlPath = Path.Combine(downloadsPath, fileName);

        XmlDocument newDoc = dataHandlers.CreateXmlDocument(lastSearchResult);
        string tempXmlPath = Path.Combine(FileSystem.CacheDirectory, "temp.xml");
        newDoc.Save(tempXmlPath);

        try
        {
            XslCompiledTransform xct = new XslCompiledTransform();
            xct.Load(dataHandlers.XslFilePath);
            xct.Transform(tempXmlPath, outputHtmlPath);
            DisplayAlert("Success", $"HTML exported to {outputHtmlPath}", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to export HTML: {ex.Message}", "OK");
        }
    }

    private string GetDownloadsPath()
    {
#if WINDOWS
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
#elif MACCATALYST
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads";
#else
        return FileSystem.AppDataDirectory;
#endif
    }

    protected override bool OnBackButtonPressed()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            bool exit = await DisplayAlert("Exit", "Do you really want to exit the application?", "Yes", "No");
            if (exit)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        });
        return true;
    }
}

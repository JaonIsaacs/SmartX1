using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartX.Models;
using SmartX.Services;

namespace SmartX.Pages.Admin;

public class TestDataModel : PageModel
{
    private readonly MockDataGenerator _generator = new();

    public List<SensorProfile> TestDevices { get; set; } = new();

    public void OnGet()
    {
        TestDevices = _generator.GetTestSensorProfiles();
    }
}
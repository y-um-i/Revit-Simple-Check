using Autodesk.Revit.UI;
using System.Reflection;

public class App : IExternalApplication
{
    public Result OnStartup(UIControlledApplication app)
    {
        string tabName = "検図ツール";

        try
        {
            app.CreateRibbonTab(tabName);
        }
        catch { }

        RibbonPanel panel = app.CreateRibbonPanel(tabName, "簡易チェック");

        PushButtonData button = new PushButtonData(
            "SimpleCheck",
            "簡易検図",
            Assembly.GetExecutingAssembly().Location,
            "Command"
        );

        panel.AddItem(button);

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication app)
    {
        return Result.Succeeded;
    }
}

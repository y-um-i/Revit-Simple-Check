using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIDocument uidoc = commandData.Application.ActiveUIDocument;
        Document doc = uidoc.Document;

        // 現在のビュー
        View view = doc.ActiveView;

        // 柱と梁を取得
        FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id)
            .WhereElementIsNotElementType()
            .OfClass(typeof(FamilyInstance));

        List<BuiltInCategory> categories = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming
        };

        using (Transaction trans = new Transaction(doc, "簡易検図"))
        {
            trans.Start();

            foreach (Element elem in collector)
            {
                if (!categories.Contains((BuiltInCategory)elem.Category.Id.IntegerValue))
                    continue;

                // Mark取得
                Parameter markParam = elem.LookupParameter("Mark");

                if (markParam == null || string.IsNullOrEmpty(markParam.AsString()))
                {
                    LocationPoint location = elem.Location as LocationPoint;
                    if (location == null) continue;

                    XYZ point = location.Point;

                    // テキスト配置
                    TextNoteOptions options = new TextNoteOptions();
                    options.HorizontalAlignment = HorizontalTextAlignment.Center;

                    TextNote textNote = TextNote.Create(
                        doc,
                        view.Id,
                        point,
                        "＝",
                        options
                    );

                    // 赤色にする
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    ogs.SetProjectionLineColor(new Color(255, 0, 0));

                    view.SetElementOverrides(textNote.Id, ogs);
                }
            }

            trans.Commit();
        }

        return Result.Succeeded;
    }
}

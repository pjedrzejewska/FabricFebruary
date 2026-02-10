using System;
using System.IO;
using System.Linq;

// get columns starting with KPI
// -----------------------------------------------------------------------------------------
var sourcePath = @"C:\\Users\\PaulinaJedrzejewska\\OneDrive - JSP Cloud GmbH\\Documents\\GIT\\EventsPrivate\\FabricFebruary\\2026\\Sorcery demo.SemanticModel\\definition\\tables\\Sales.tmdl";
var sourceLines = File.ReadAllLines(sourcePath);
var kpiColumns = new List<string>();

Console.WriteLine("Collecting columns starting with KPI...");
for (int i = 0; i < sourceLines.Length; i++)
{
    var line = sourceLines[i].Trim();
    if (line.StartsWith("column 'KPI"))
    {
        var colName = line.Substring(8, line.Length - 9); // skip "column " and ''
        kpiColumns.Add(colName); // add column
        Console.WriteLine($"Line {i+1}: {colName}");
    }
}


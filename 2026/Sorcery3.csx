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

// Add Measures from columns
// -----------------------------------------------------------------------------------------
var destinationPath = "C:\\Users\\PaulinaJedrzejewska\\OneDrive - JSP Cloud GmbH\\Documents\\GIT\\EventsPrivate\\FabricFebruary\\2026\\Sorcery demo.SemanticModel\\definition\\tables\\_Measures.tmdl";

// read all destinationLines
var destinationLines = File.ReadAllLines(destinationPath).ToList();

// find the first line that starts with "partition" to insert new measures before that line
var index = destinationLines.FindIndex(line => line.TrimStart().StartsWith("partition"));



// Collect calculation items

var calculationPath = @"C:\\Users\\PaulinaJedrzejewska\\OneDrive - JSP Cloud GmbH\\Documents\\GIT\\EventsPrivate\\FabricFebruary\\2026\\Sorcery demo.SemanticModel\\definition\\tables\\Time Intelligence.tmdl";
var calculationItems = new List<string>();

Console.WriteLine("Collecting calculation items...");
Console.WriteLine("");
foreach (var line in File.ReadAllLines(calculationPath))
{
    var text = line.Trim();
    if (text.StartsWith("calculationItem "))
    {
        var part = text.Substring(16).Split('=')[0].Trim(); // after "calculationItem "
        var calculationItem = part.Trim('\''); // remove quotes if present
        calculationItems.Add(calculationItem);

        Console.WriteLine(calculationItem);
        Console.WriteLine("");
    }
}


// build all new measures (cartesian product)
Console.WriteLine("Creating measures...");
Console.WriteLine("");
var toInsert = new List<string>();
foreach (var col in kpiColumns)
{
    foreach (var calc in calculationItems)
    {
        var newMeasureName = $"{col} {calc}";

        // DAX object full name for the base measure we created earlier:
        var daxFull = $"[{col}]";

        var measureBlock =
$@"    /// CALCULATE({daxFull}, 'Time Intelligence'[TimeIntelligence]=""{calc}"")
    measure '{newMeasureName}' = CALCULATE({daxFull}, 'Time Intelligence'[TimeIntelligence]=""{calc}"")
        displayFolder: {calc}";

        // blank line + block (keeps spacing like your script)
        destinationLines.Insert(index, "");
        destinationLines.Insert(index, measureBlock);
        Console.WriteLine(measureBlock);
        Console.WriteLine("");
    }
}

// insert and save
File.WriteAllLines(destinationPath, destinationLines);
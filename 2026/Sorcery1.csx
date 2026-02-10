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

// add measures to the file
Console.WriteLine("creating measures...");

// loop through columns
foreach (var col in kpiColumns)
{
    //create string for measure
    var measure = 
$@"    /// SUM('Sales'[{col}])
    measure '{col}' = SUM('Sales'[{col}])
        displayFolder: BASE";

    destinationLines.Insert(index, "");
    destinationLines.Insert(index, measure);
    Console.WriteLine(measure);
    Console.WriteLine(); // blank line between measures
}


// insert new line before the partition line
File.WriteAllLines(destinationPath, destinationLines);

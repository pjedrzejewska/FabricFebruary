/* Generate measures from columns */

var measuresTable = Model.Tables["_Measures"];
var kpiSourceTable = Model.Tables["Sales"];

foreach(var column in kpiSourceTable.Columns)
{
    if (column.Name.Contains("KPI ")) 
    {
        measuresTable.AddMeasure(
        column.Name + " BASE",                                    // Name
        "SUM(" + column.DaxObjectFullName + ")",        // DAX expression
        "BASE"                                          // Display Folder
        );
    }
}

/* Delete all measures */

var measuresTable = Model.Tables["_Measures"];
foreach(var measure in measuresTable.Measures.ToList())
{
    measure.Delete();
}


/* Hide Measures */

var measuresTable = Model.Tables["_Measures"];
foreach(var m in measuresTable.Measures.Where(m => (m.DisplayFolder.Contains("BASE"))).ToList())
{
    m.IsHidden = true;
}

/* Create measures from calculation items */

var measuresTable = Model.Tables["_Measures"];
var baseMeasures = measuresTable.Measures.Where(m => (m.DisplayFolder=="BASE")).ToList();

foreach(var measure in baseMeasures)
{
    
    foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
    {
        // add measure
        measuresTable.AddMeasure(
        measure.Name + " " + calcItem.Name,                              // Name
        "CALCULATE(" + measure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
        calcItem.Name                         // Display Folder
    );
    }
}

/* Format Measures */

var measuresTable = Model.Tables["_Measures"];
foreach(var m in measuresTable.Measures.Where(m => (m.DisplayFolder.Contains("%"))).ToList())
{
    m.FormatString = "#.##%";
}

/* Script */

var measuresTable = Model.Tables["_Measures"];
var kpiSourceTable = Model.Tables["Sales"];

foreach(var measure in measuresTable.Measures.ToList())
{
    measure.Delete();
}

foreach(var column in kpiSourceTable.Columns)
{
    if (column.Name.Contains("KPI ")) 
    {   
        var daxFunction = "SUM(";
        
        if (column.Name.Contains("Unit ")) 
        {
            daxFunction = "AVERAGE(";
        }
        
        var baseMeasure = measuresTable.AddMeasure(
                column.Name + " BASE",                               // Name
                daxFunction + column.DaxObjectFullName + ")",        // DAX expression
                "BASE"                                               // Display Folder
                );
        baseMeasure.IsHidden = true;

        // get correct measure name
        var indexOfBase = baseMeasure.Name.IndexOf("BASE");
        var measureNameWithoutBase = baseMeasure.Name.Substring(0, indexOfBase -5);
        var correctMeasureName = measureNameWithoutBase.Remove(0,3);

        foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
        {
            var newCalcMeasure = measuresTable.AddMeasure(
            correctMeasureName + " " + calcItem.Name,                              // Name
            "CALCULATE(" + baseMeasure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
            calcItem.Name                         // Display Folder
            );

            if(calcItem.Name.Contains("%")) 
            {
                newCalcMeasure.FormatString = "#.##%";
            }
            else if(column.Name.Contains("USD"))
            {
                newCalcMeasure.FormatString = "#.##$";
            }

            newCalcMeasure.Description = "BASE: " + baseMeasure.Expression + @"
        
CALC: " + calcItem.Expression;
        }

    }
}
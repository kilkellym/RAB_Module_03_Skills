#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;

#endregion

namespace RAB_Module_03_Skills
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 2. Create instances of class - v1
            Building theater = new Building("Grand Opera House", "5 Main Street", 4, 35000);
            Building hotel = new Building("Fancy Hotel", "10 Main Street", 10, 100000);
            Building office = new Building("Big Office Building", "15 Main Street", 15, 150000);

            // 3. Create list of buildings
            List<Building> buildingList = new List<Building>();
            buildingList.Add(theater);
            buildingList.Add(hotel);
            buildingList.Add(office);
            buildingList.Add(new Building("Hospital", "20 Main Street", 20, 350000));

            // 6. Create instance of class and use method
            Neighborhood downtown = new Neighborhood("Downtown", "Middletown", "CT", buildingList);

            TaskDialog.Show("Test", $"There are {downtown.GetBuildingCount()} " +
                $"buildings in the {downtown.Name} neighborhood.");

            // 7. Working with Rooms
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms);

            // 8. Insert family
            FamilySymbol curFS = Utils.GetFamilySymbolByName(doc, "Desk", "60\" x 30\"");

            using(Transaction t = new Transaction(doc))
            {
                t.Start("Insert family into room");

                // 9. Activate family symbol
                curFS.Activate();

                foreach (SpatialElement room in collector)
                {
                    LocationPoint loc = room.Location as LocationPoint;
                    XYZ roomPoint = loc.Point as XYZ;

                    FamilyInstance curFI = doc.Create.NewFamilyInstance(roomPoint, curFS, StructuralType.NonStructural);

                    // 10. Get parameter value
                    string name = Utils.GetParameterValueAsString(room, "Department");

                    // 11. Set parameter values
                    Utils.SetParameterValue(room, "Ceiling Finish", "ACT");
                }
                t.Commit();

                // 11. string splitting
                string myLine = "one, two, three, four, five";
                string[] splitLine = myLine.Split(',');
                TaskDialog.Show("Test", splitLine[0].Trim());
                TaskDialog.Show("Test", splitLine[3].Trim());
            }

                return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }



    public class Building
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int NumFloors { get; set; }
        public double Area { get; set; }

        // 3. Add constructor to class
        public Building(string _name, string _address, int _numFloors, double _area)
        {
            Name = _name;
            Address = _address;
            NumFloors = _numFloors;
            Area = _area;
        }
    }

    // 4. Define dynamic class #2
    public class Neighborhood
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public List<Building> BuildingList { get; set; }
        public Neighborhood(string _name, string _city, string _state, List<Building> _buildings)
        {
            Name = _name;
            City = _city;
            State = _state;
            BuildingList = _buildings;
        }

        // 5. Add method to class
        public int GetBuildingCount()
        {
            return BuildingList.Count;
        }
    }
}

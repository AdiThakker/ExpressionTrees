using System;
using System.Reflection;

namespace ExpressionTrees.Serialization
{
    [Serializable]
    public class Equipment
    {
        public string Name { get; set; }
    }

    public class EquipmentLogic
    {
        public bool AddItem(Equipment equipment)
        {
            Console.WriteLine(equipment.Name + " Added");
            
            // Capture
            var methodInfo = MethodInfo.GetCurrentMethod() as MethodInfo;
            ExpressionUtility.Save(methodInfo, equipment);

            return true;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PDMS.Web.Tasks
{
    public interface IPISTaskBase
    {

    }

    public class PISTaskManager
    {
        // get available tasks
        public static List<Type> GetAvailabeTasks()
        {
            var taskInterfaceName = typeof(IPISTaskBase);
            var taskClassType = Assembly.GetExecutingAssembly().GetTypes().Where(t => taskInterfaceName.IsAssignableFrom(t));
            return taskClassType.OrderBy(t => t.FullName).ToList();
        }
    }
}
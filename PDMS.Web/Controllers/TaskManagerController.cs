using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class TaskManagerController : Controller
    {
        public ActionResult Index()
        {
            List<Type> taskTypes = Tasks.PISTaskManager.GetAvailabeTasks();
            return View(taskTypes);
        }

        public ActionResult ExecuteTask(string action, string taskName)
        {
            // find class & method
            var str = taskName.Split('.');
            string className = str[0];
            string methodName = str[1];
            // get type and method info
            var taskInterfaceName = typeof(Tasks.IPISTaskBase);
            var taskClassType = Assembly.GetExecutingAssembly().GetTypes().Where(t => taskInterfaceName.IsAssignableFrom(t) && t.Name == className).FirstOrDefault();
            if (taskClassType != null)
            {
                MethodInfo myMethod = taskClassType.GetMethod(methodName);
                List<object> myParams = new List<object>();
                var @switch = new Dictionary<Type, Action>
                {
                        { typeof(int), () => {
                            myParams.Add(0);
                        } },
                        { typeof(string), () => {
                            myParams.Add("");
                        } },
                        { typeof(IJobCancellationToken), () => {
                            myParams.Add(JobCancellationToken.Null);
                        } },
                };
                foreach (ParameterInfo p in myMethod.GetParameters())
                {
                    if (@switch.ContainsKey(p.ParameterType))
                        @switch[p.ParameterType]();
                    else
                        myParams.Add(null);
                }
                if (myMethod != null)
                    // execute
                    switch (action)
                    {
                        case "Run":
                            BackgroundJob.Enqueue(() => myMethod.Invoke(null, myParams.ToArray()));
                            break;
                        case "Schedule":
                            BackgroundJob.Schedule(() => myMethod.Invoke(null, myParams.ToArray()), TimeSpan.FromDays(1));
                            break;
                        case "Recurring":
                            string myTaskName = methodName + Guid.NewGuid().ToString().Replace("-", "").Remove(6);
                            RecurringJob.AddOrUpdate(myTaskName, () => myMethod.Invoke(null, myParams.ToArray()), string.Format("{0} {1} * * *", 30, 3), TimeZoneInfo.Local);
                            break;
                        default:
                            break;
                    }
            }
            return Content("{success:true}", "application/json");
        }
    }
}
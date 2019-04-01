using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class OtherSystemController : Controller
    {
        // GET: OtherSystem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PlmSystem()
        {
            return Redirect("http://wxiplm01.corp.jabil.org/Windchill/app/#ptc1/homepage");
        }

        public ActionResult WorkOvertimeSystem()
        {
            return Redirect("http://cnctug0hrmweb01/CN51/FirstLogin.aspx");
        }

        public ActionResult HRISSytem()
        {
            return Redirect("http://twtchg0km01/hris/#/HRIS/DataCenter/EmployeeInfo/IntroduceNewHire.aspx");
        }

        public ActionResult SFSystem()
        {
            return Redirect("https://performancemanager4.successfactors.com/login");
        }

        public ActionResult ITSystem()
        {
            return Redirect("http://ctuweb01/");
        }
    }
}
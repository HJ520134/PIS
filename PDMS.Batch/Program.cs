using PDMS.Batch.BatchProcess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity;
using PDMS.Common.Constants;
using System.Reflection;

namespace PDMS.Batch
{
    class Program
    {
        static void Main(string[] args)
        {
            //UnityContainer container = new UnityContainer();
            //UnityConfigurationSection configuration = (UnityConfigurationSection)ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            //configuration.Configure(container, ConstConstants.WebContainer);
            //ABSExec exec = container.Resolve<ABSExec>();

            BatchNotice notice = new BatchNotice();
            var keyList = ConfigurationManager.AppSettings.AllKeys.Where(m => m.Contains("Batch")).ToList();
            foreach (var keyItem in keyList)
            {
                //获取WebConfig Value
                var className = ConfigurationManager.AppSettings[keyItem].ToString();
                //创建实例
                var instance = (ABSExec)Assembly.Load(ConstConstants.AssemblyName).CreateInstance(className);
                notice.Attend(instance);
            }
            notice.Run();
        }
    }
}

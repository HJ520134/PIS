using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Core.Authentication
{
    public class ValidateUser
    {
        ISettingsService settingsService;

        public ValidateUser(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }
        public bool LDAPValidate(string userName, string password, int isEmployee)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            DirectoryEntry entry = new DirectoryEntry(ConfigurationManager.AppSettings["LDAPPath"].ToString(), userName, password);

            try
            {
                if (isEmployee == 0)
                {
                    string objectSid = (new SecurityIdentifier((byte[])entry.Properties["objectSid"].Value, 0).Value);
                    return objectSid != null;
                }
                else
                {
                    bool flag = false;
                    flag=settingsService.EmployeeLogin(userName, JGP.Common.PasswordUtil.EncryptionHelper.Encrypt(password, ""));
                    if (!flag)
                    {
                        flag = settingsService.EmployeeLogin(userName, eTransfer.PasswordUtil.EncryptionHelper.Encrypt(password));
                    }
                    return flag;
                }
            }
            catch // directory services COMException
            {
                return false;
            }
            finally // release unmanaged resource
            {
                entry.Dispose();
            }
        }

        public bool LDAPValidateByMHFlag(string userName, string password, int isEmployee)
        {
            bool flag = false;
            flag = settingsService.MH_FlagLogin(userName, JGP.Common.PasswordUtil.EncryptionHelper.Encrypt(password, ""));
            if (!flag)
            {
                flag = settingsService.MH_FlagLogin(userName, eTransfer.PasswordUtil.EncryptionHelper.Encrypt(password));
            }
            return flag;
            //return settingsService.MH_FlagLogin(userName, JGP.Common.PasswordUtil.EncryptionHelper.Encrypt(password, ""));
        }
    }
}

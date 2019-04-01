using PDMS.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;

namespace PDMS.Data.Repository
{
    public class SystemRoleFunctionSubRepository : RepositoryBase<System_Role_FunctionSub>, ISystemRoleFunctionSubRepository
    {
        public SystemRoleFunctionSubRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<SubFunction> QueryRoleSubFunctionsByRoleUIDAndFunctionUID(int role_uid, int function_uid)
        {
            var sql = @"SELECT
                        0 as System_Role_FunctionSub_UID, 
                        System_FunctionSub_UID,
                        Function_UID, 
                        Sub_Fun, 
                        Sub_Fun_Name,
                        cast(0 as bit) as [Grant]
                        FROM   [dbo].[System_FunctionSub]
                        WHERE Function_UID = @Function_UID

                        EXCEPT

                        SELECT 
                        0 as System_Role_FunctionSub_UID, 
                        a.System_FunctionSub_UID, 
                        a.Function_UID, 
                        Sub_Fun, 
                        Sub_Fun_Name,
                        cast(0 as bit) as [Grant]
                        FROM   [dbo].[System_FunctionSub] AS a
                        inner JOIN [dbo].[System_Role_FunctionSub] AS b ON a.System_FunctionSub_UID = b.[System_FunctionSub_UID]
                        inner JOIN [dbo].[System_Role_Function] AS c ON (b.[System_Role_Function_UID] = c.[System_Role_Function_UID]) 
                        WHERE a.Function_UID = @Function_UID AND c.[Role_UID] = @Role_UID

                        UNION 

                        SELECT 
                        b.System_Role_FunctionSub_UID,
                        a.System_FunctionSub_UID, 
                        a.Function_UID, 
                        Sub_Fun, 
                        Sub_Fun_Name,
                        b.Sub_Flag as [Grant]
                        FROM   [dbo].[System_FunctionSub] AS a
                        inner JOIN [dbo].[System_Role_FunctionSub] AS b ON a.System_FunctionSub_UID = b.[System_FunctionSub_UID]
                        inner JOIN [dbo].[System_Role_Function] AS c ON (b.[System_Role_Function_UID] = c.[System_Role_Function_UID]) 
                        WHERE a.Function_UID = @Function_UID AND c.[Role_UID] = @Role_UID";

            var query = DataContext.Database.SqlQuery<SubFunction>(sql, new SqlParameter("Role_UID", role_uid), new SqlParameter("Function_UID", function_uid));

            return query.AsQueryable();
        }
    }

    public interface ISystemRoleFunctionSubRepository : IRepository<System_Role_FunctionSub>
    {
        IQueryable<SubFunction> QueryRoleSubFunctionsByRoleUIDAndFunctionUID(int role_uid, int function_uid);
    }
}

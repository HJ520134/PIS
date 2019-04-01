using PDMS.Data;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IGL_BuildPlanService : IBaseSevice<GL_BuildPlan, GL_BuildPlanDTO, GL_BuildPlanModelSearch>
    {
    }
    public class GL_BuildPlanService : BaseSevice<GL_BuildPlan, GL_BuildPlanDTO, GL_BuildPlanModelSearch>, IGL_BuildPlanService
    {
        private readonly IGL_BuildPlanRepository buildPlanRepository;
        public GL_BuildPlanService(IGL_BuildPlanRepository buildPlanRepository) : base(buildPlanRepository)
        {
            this.buildPlanRepository = buildPlanRepository;
        }
    }
}

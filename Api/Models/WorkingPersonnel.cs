using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    /// <summary>
    /// 工作人员实体类
    /// </summary>
    [SugarTable("sys_working_personnel")]
    public class WorkingPersonnel : BaseModel
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { set; get; }
        public string Name { set; get; }
        public string LoginName { set; get; }
        public string Password { set; get; }
        public int RoleId { set; get; }
        public int? UserId { set; get; }
        public bool Visible { set; get; } = true;
        public bool IsDel { set; get; } = false;
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    [SugarTable("sys_role_menu")]
    public class RoleMenu
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { set; get; }
        public int RoleID { set; get; }
        public string TreeID { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class AdminLoginResponseViewModel
    {
        public int? UserId { get; set; }
        /// <summary>
        /// 工作人员姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId { get; set; }
    }
}

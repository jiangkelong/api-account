using Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class MemberPageListParmModel: PageParm
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string QueryWords { set; get; }
    }
}

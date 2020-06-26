using Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class AccountLogPageListParmModel : PageParm
    {
        //以下查询字段
        public string MemberNo { set; get; }
        public string MemberName { set; get; }
        public string MakeMan { set; get; }
        public string Type { set; get; }
        public string BeginDate { set; get; }
        public string EndDate { set; get; }
    }
}

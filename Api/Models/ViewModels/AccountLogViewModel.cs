using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class AccountLogViewModel
    {
        public string MemberNo { set; get; }
        public string MemberName { set; get; }
        public string MakeMan { set; get; }
        public decimal? Money { set; get; }
        public decimal? Num { set; get; }
        public string Type { set; get; }
        public string CreatedOn { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class CustomerBillViewModel
    {
        public decimal? Money { set; get; }
        public decimal? Num { set; get; }
        public string Type { set; get; }
        public string CreatedOn { set; get; }
    }
}

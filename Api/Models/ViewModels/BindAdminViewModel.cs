using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class BindAdminViewModel
    {
        public string OpenId { set; get; }
        public string LoginName { set; get; }
        public string Password { set; get; }
    }
}

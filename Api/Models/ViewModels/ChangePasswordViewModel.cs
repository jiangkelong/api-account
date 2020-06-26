using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string OldPassword { set; get; }

        public string NewPassword { set; get; }
    }
}

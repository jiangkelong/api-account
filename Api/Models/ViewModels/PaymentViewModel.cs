using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class PaymentViewModel
    {
        /// <summary>
        /// 收款人ID
        /// </summary>
        public int MakeManId { set; get; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal Money { set; get; }
        /// <summary>
        /// 付款数量
        /// </summary>
        public decimal Num { set; get; }
        /// <summary>
        /// 会员密码
        /// </summary>
        public string Password { set; get; }
    }
}

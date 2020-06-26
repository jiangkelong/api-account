using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.ViewModels
{
    public class MemberViewModel
    {
        public int MemberId { set; get; }
        /// <summary>
        /// 会员编号
        /// </summary>
        public string MemberNo { set; get; }
        /// <summary>
        /// 会员名称
        /// </summary>
        public string MemberName { set; get; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Tel { set; get; }
        public int? UserId { set; get; }
        /// 账户余额-金额（元）
        /// </summary>
        public decimal Balance_Money { set; get; }
        /// <summary>
        /// 账户余额-数量（桶）
        /// </summary>
        public decimal Balance_Num { set; get; }
    }
}

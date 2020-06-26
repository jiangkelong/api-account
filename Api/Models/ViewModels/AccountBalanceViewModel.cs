namespace Api.Models.ViewModels
{
    public class AccountBalanceViewModel
    {
        public int? UserId { set; get; }
        /// <summary>
        /// 会员编号
        /// </summary>
        public string MemberNo { get; set; }
        public string MemberName { get; set; }
        /// <summary>
        /// 余额-金额（元）
        /// </summary>
        public decimal Balance_Money { get; set; }
        /// <summary>
        /// 余额-数量（桶）
        /// </summary>
        public decimal Balance_Num { get; set; }
    }
}

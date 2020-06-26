namespace Api.Models.ViewModels
{
    /// <summary>
    /// 充值/扣费
    /// </summary>
    public class RechargeViewModel
    {
        public int MemberId { set; get; }
        public int MakeManId { set; get; }
        public decimal Money { set; get; }
        public decimal Num { set; get; }
        /// <summary>
        /// recharge or charging
        /// </summary>
        public string Type { set; get; }
    }
}

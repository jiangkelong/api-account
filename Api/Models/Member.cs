using SqlSugar;

namespace Api.Models
{
    /// <summary>
    /// 会员实体类
    /// </summary>
    [SugarTable("biz_member")]
    public class Member : BaseModel
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int MemberId { set; get; }
        /// <summary>
        /// 会员编号
        /// </summary>
        public string MemberNo { set; get; }
        public string Password { set; get; } = "888888";
        /// <summary>
        /// 会员名称
        /// </summary>
        public string MemberName { set; get; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 账户余额-金额（元）
        /// </summary>
        public decimal Balance_Money { set; get; }
        /// <summary>
        /// 账户余额-数量（桶）
        /// </summary>
        public decimal Balance_Num { set; get; }
        public bool IsDel { set; get; } = false;
    }
}

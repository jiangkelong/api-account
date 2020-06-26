using SqlSugar;

namespace Api.Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [SugarTable("sys_user")]
    public class User :BaseModel
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int UserId { set; get; }

        public string OpenId { set; get; }
        public int? MemberId { set; get; }
    }
}

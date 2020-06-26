using SqlSugar;

namespace Api.Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [SugarTable("sys_role")]
    public class Role
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int RoleId { set; get; }
        public string RoleName { set; get; }
    }
}

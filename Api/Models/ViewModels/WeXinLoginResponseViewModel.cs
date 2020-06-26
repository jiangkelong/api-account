namespace Api.Models.ViewModels
{
    public class WeXinLoginResponseViewModel
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public int? UserId { get; set; }

        public string OpenId { get; set; }
        /// <summary>
        /// 是否为会员
        /// </summary>
        public bool IsMember { get; set; } = false;
        /// <summary>
        /// 是否为工作人员
        /// </summary>
        public bool IsAdmin { get; set; } = false;
        /// <summary>
        /// 工作人员姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

    }
}

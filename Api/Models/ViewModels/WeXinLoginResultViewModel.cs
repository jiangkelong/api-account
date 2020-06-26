namespace Api.Models.ViewModels
{
    public class WeXinLoginResultViewModel
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 会话密钥
        /// </summary>
        public string Session_Key { get; set; }
        /// <summary>
        /// 用户在开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回，详见UnionID 机制说明。
        /// </summary>
        public string Unionid { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}

using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> AddAsync(User parm);
        /// <summary>
        /// 小程序登录
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<WeXinLoginResponseViewModel>> wxLoginAsync(string openid);
        Task<ApiResult<AdminLoginResponseViewModel>> adminLoginAsync(AdminLoginViewModel parm);
    }
}

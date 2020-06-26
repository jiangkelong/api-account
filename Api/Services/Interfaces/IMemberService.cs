using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IMemberService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> AddAsync(Member parm);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        Task<ApiResult<string>> AddListAsync(List<Member> list);
        /// <summary>
        /// 假删除,将memberno和openid改为null，避免唯一键重复
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> DeleteAsync(int id);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ModifyAsync(Member parm);
        /// <summary>
        /// 根据openid查询一条信息
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Member>> GetByUserIdAsync();
        /// <summary>
        /// 根据memberNo查询一条信息
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Member>> GetByMemberNoAsync(string memberNo);
        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Page<MemberViewModel>>> GetPageListAsync(MemberPageListParmModel pageParm);
        /// <summary>
        /// 根据openid查询余额
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<AccountBalanceViewModel>> GetBalanceByUserIdAsync(int? userId = null);
        /// <summary>
        /// 绑定会员卡
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        Task<ApiResult<AccountBalanceViewModel>> BindMemberCardAsync(BindMemberCardViewModel parm);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ResetPasswordAsync(int id);
        Task<ApiResult<string>> ChangePasswordCusAsync(string newPassword);
    }
}

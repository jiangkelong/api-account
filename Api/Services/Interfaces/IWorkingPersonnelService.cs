using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IWorkingPersonnelService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> AddAsync(WorkingPersonnel parm);
        /// <summary>
        /// 假删除，将loginname和openid改为null，避免唯一键重复
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> DeleteAsync(int id);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ModifyAsync(WorkingPersonnel parm);
        /// <summary>
        /// 根据openid查询一条信息
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<WorkingPersonnel>> GetByOpenIdAsync();
        /// <summary>
        /// 根据id查询一条信息
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<WorkingPersonnel>> GetByIdAsync(int id);
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<List<WorkingPersonnelViewModel>>> GetListAsync();
        /// <summary>
        /// 绑定工作人员账号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        Task<ApiResult<WorkingPersonnelRoleViewModel>> BindAdminAsync(BindAdminViewModel parm);
        Task<ApiResult<string>> ChangePasswordAsync(ChangePasswordViewModel parm);
    }
}

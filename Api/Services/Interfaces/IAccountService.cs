using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// 充值、收款(后台操作)
        /// 变动账户余额
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ChangeBalanceAsync(RechargeViewModel parm);
        /// <summary>
        /// 分页查询余额变动列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Page<AccountLogViewModel>>> GetLogPageListAsync(AccountLogPageListParmModel pageParm);
        ApiResult<List<AccountLogViewModel>> GetLogList(AccountLogPageListParmModel pageParm);
        /// <summary>
        /// 客户获取账单
        /// </summary>
        /// <param name="pageParm"></param>
        /// <returns></returns>
        Task<ApiResult<Page<CustomerBillViewModel>>> GetBillPageListAsync(PageParm pageParm);
        /// <summary>
        /// 客户付款
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        Task<ApiResult<string>> PaySubmitAsync(PaymentViewModel parm);
    }
}

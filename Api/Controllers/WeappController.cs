using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using Api.Services.Implements;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AuthorizationFilter]
    public class WeappController : ControllerBase
    {
        IUserService _iUserService = new UserService();
        IMemberService _iMemberService = new MemberService();
        IWorkingPersonnelService _iWorkingPersonnelService = new WorkingPersonnelService();
        IRoleService _iRoleService = new RoleService();
        IAccountService _iAccountService = new AccountService();
        IWxService _iWxService = new WxService();

        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="js_code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> wxLogin(string js_code)
        {
            string param = $"?appid={WxConfig.APPID}&secret={WxConfig.APPSECRET}&js_code={js_code}&grant_type=authorization_code";
            string url = "https://api.weixin.qq.com/sns/jscode2session" + param;

            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler))
            {
                //await异步等待回应
                var response = await http.GetAsync(url);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                var a = response.StatusCode;

                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                string responseContent = await response.Content.ReadAsStringAsync();
                var resultModel = JsonConvert.DeserializeObject<WeXinLoginResultViewModel>(responseContent);

                var res = await _iUserService.wxLoginAsync(resultModel.OpenId);
                //返回结果
                return Ok(res);
            }
        }
        public async Task<IActionResult> CustomerChangePassword(string newPassword)
        {
            var res = await _iMemberService.ChangePasswordCusAsync(newPassword);
            return Ok(res);
        }
        /// <summary>
        /// 获取账户
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetBalance()
        {
            var account = await _iMemberService.GetBalanceByUserIdAsync();
            return Ok(account);
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetRoleList()
        {
            var res = await _iRoleService.GetListAsync();
            return Ok(res);
        }
        /// <summary>
        /// 绑定会员卡
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> BindMemberCard(BindMemberCardViewModel parm)
        {
            var account = await _iMemberService.BindMemberCardAsync(parm);
            return Ok(account);
        }
        /// <summary>
        /// 绑定工作人员账号
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> BindAdmin(BindAdminViewModel parm)
        {
            var res = await _iWorkingPersonnelService.BindAdminAsync(parm);
            return Ok(res);
        }
        /// <summary>
        /// 获取工作人员账号列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetWorkingPersonnelList()
        {
            var res = await _iWorkingPersonnelService.GetListAsync();
            return Ok(res);
        }
        /// <summary>
        /// 保存工作人员账号
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveWorkingPersonnel(WorkingPersonnel parm)
        {
            var res = new ApiResult<string>(); 
            if(parm.Id==0)
                res= await _iWorkingPersonnelService.AddAsync(parm);
            else
                res = await _iWorkingPersonnelService.ModifyAsync(parm);
            return Ok(res);
        }
        /// <summary>
        /// 删除工作人员账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DelWorkingPersonnel(int id)
        {
            var res = await _iWorkingPersonnelService.DeleteAsync(id);
            return Ok(res);
        }
        
        /// <summary>
        /// 获取会员列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetMemberList(MemberPageListParmModel pageParm)
        {
            var res = await _iMemberService.GetPageListAsync(pageParm);
            return Ok(res);
        }
        /// <summary>
        /// 保存会员信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveMember(Member parm)
        {
            var res = new ApiResult<string>();
            if (parm.MemberId == 0)
                res = await _iMemberService.AddAsync(parm);
            else
                res = await _iMemberService.ModifyAsync(parm);
            return Ok(res);
        }
        /// <summary>
        /// 删除会员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DelMember(int id)
        {
            var res = await _iMemberService.DeleteAsync(id);
            return Ok(res);
        }
        /// <summary>
        /// 重置会员密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ResetMemberPassword(int id)
        {
            var res = await _iMemberService.ResetPasswordAsync(id);
            return Ok(res);
        }
        /// <summary>
        /// 后台操作充值、收款
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangeBalance(RechargeViewModel parm)
        {
            var res = await _iAccountService.ChangeBalanceAsync(parm);
            return Ok(res);
        }
        /// <summary>
        /// 后台获取账目流水
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetChargeLogList(AccountLogPageListParmModel pageParm)
        {
            var res = await _iAccountService.GetLogPageListAsync(pageParm);
            return Ok(res);
        }
        /// <summary>
        /// 客户获取账单
        /// </summary>
        /// <param name="pageParm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetBillList(PageParm pageParm)
        {
            var res = await _iAccountService.GetBillPageListAsync(pageParm);
            return Ok(res);
        }
        /// <summary>
        /// 客户付款
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<IActionResult> PaySubmit(PaymentViewModel parm)
        {
            var res = await _iAccountService.PaySubmitAsync(parm);
            return Ok(res);
        }
        public IActionResult CreateQRCode()
        {
            string page = "pages/account/pay/pay";
            var res = _iWxService.CreateQRCode(page);
            return Ok(res);
        }
    }
}
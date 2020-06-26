using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common;
using Api.Models.ViewModels;
using Api.Services.Implements;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AuthorizationFilter]
    public class AdminController : ControllerBase
    {
        IUserService _iUserService = new UserService();
        IAdminService _iAdminService = new AdminService();
        IWorkingPersonnelService _iWorkingPersonnelService = new WorkingPersonnelService();
        [AllowAnonymous]
        public async Task<IActionResult> Login(AdminLoginViewModel parm)
        {
            var res = await _iUserService.adminLoginAsync(parm);
            return Ok(res);
        }
        public async Task<IActionResult> GetMenu()
        {
            var res = await _iAdminService.GetRoleMenuAsync();
            return Ok(res);
        }
        /// <summary>
        /// 工作人员自主修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel parm)
        {
            var res = await _iWorkingPersonnelService.ChangePasswordAsync(parm);
            return Ok(res);
        }
    }
}
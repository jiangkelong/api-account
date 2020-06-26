using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using Api.Services.Interfaces;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Services.Implements
{
    public class UserService : DbContext, IUserService
    {
        public async Task<User> AddAsync(User parm)
        {
            User u = new User();
            try
            {
                parm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                u = await Db.Insertable(parm).ExecuteReturnEntityAsync();
            }
            catch
            {

            }
            return u;
        }

        public async Task<ApiResult<AdminLoginResponseViewModel>> adminLoginAsync(AdminLoginViewModel parm)
        {
            var res = new ApiResult<AdminLoginResponseViewModel>() { statusCode = 200 };
            try
            {
                var wp = await Db.Queryable<WorkingPersonnel>().Where(it => it.LoginName == parm.loginName && it.Password == parm.password).SingleAsync();
                if (wp==null)
                {
                    res.success = false;
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = "账号或密码错误";
                }
                else if(wp.UserId==null)
                {
                    res.success = false;
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = "请先在小程序绑定";
                }
                else
                {
                    var model = await Db.Queryable<WorkingPersonnel, Role>((t1, t2) => new object[]
                           {
                            JoinType.Inner,t1.RoleId == t2.RoleId
                           })
                        .Where((t1, t2) => t1.UserId == wp.UserId)
                        .Select<AdminLoginResponseViewModel>().SingleAsync();

                    res.data = model;
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<WeXinLoginResponseViewModel>> wxLoginAsync(string openid)
        {
            var res = new ApiResult<WeXinLoginResponseViewModel>() { statusCode = 200 };
            var model = new WeXinLoginResponseViewModel();
            model.OpenId = openid;
            try
            {
                var user = await Db.Queryable<User>().Where(it => it.OpenId == openid).SingleAsync();
                if (user != null)
                {
                    model.UserId = user.UserId;
                    model.IsMember = user.MemberId != null;
                    var wp = await Db.Queryable<WorkingPersonnel, Role>((t1, t2) => new object[]
                         {
                        JoinType.Inner,t1.RoleId == t2.RoleId
                         }).Where((t1, t2) => t1.UserId == user.UserId)
                        .Select((t1, t2) => new
                        {
                            t1.Name,
                            t1.RoleId,
                            t2.RoleName
                        }).SingleAsync();
                    if (wp != null)
                    {
                        model.IsAdmin = true;
                        model.Name = wp.Name;
                        model.RoleId = wp.RoleId;
                        model.RoleName = wp.RoleName;
                    }
                }
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            res.data = model;
            return res;
        }
    }
}

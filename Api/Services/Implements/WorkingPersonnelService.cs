using Api.Common;
using Api.Models;
using Api.Models.ViewModels;
using Api.Services.Interfaces;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Implements
{
    public class WorkingPersonnelService : DbContext, IWorkingPersonnelService
    {
        public async Task<ApiResult<string>> AddAsync(WorkingPersonnel parm)
        {
            var res = new ApiResult<string>() {data="1", statusCode = 200 };
            try
            {
                parm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await Db.Insertable(parm).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<WorkingPersonnelRoleViewModel>> BindAdminAsync(BindAdminViewModel parm)
        {
            var res = new ApiResult<WorkingPersonnelRoleViewModel>() { statusCode = 200 };
            try
            {
                if (!await Db.Queryable<WorkingPersonnel>().Where(it => it.LoginName == parm.LoginName && it.Password == parm.Password).AnyAsync())
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "账号或密码错误";
                }
                else if (await Db.Queryable<WorkingPersonnel>().Where(it => it.LoginName == parm.LoginName && it.Password == parm.Password && SqlFunc.HasValue(it.UserId)).AnyAsync())
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "该账号已绑定其它用户";
                }
                else
                {
                    //查询或插入user，返回实体
                    IUserService iu = new UserService();
                    var user = await Db.Queryable<User>().Where(it => it.OpenId == parm.OpenId).SingleAsync();
                    if (user == null)
                        user = await iu.AddAsync(new User { OpenId = parm.OpenId });

                    await Db.Updateable<WorkingPersonnel>()
                    .SetColumns(it => new WorkingPersonnel() { UserId = user.UserId })
                    .Where(it => it.LoginName == parm.LoginName).ExecuteCommandAsync();

                    res.data = await Db.Queryable<WorkingPersonnel, Role>((t1, t2) => new object[]
                         {
                            JoinType.Inner,t1.RoleId == t2.RoleId
                         })
                        .Where((t1, t2) => t1.UserId == user.UserId)
                        .Select((t1, t2) => new WorkingPersonnelRoleViewModel
                        {
                            UserId = t1.UserId,
                            Name = t1.Name,
                            RoleId = t1.RoleId,
                            RoleName = t2.RoleName
                        }).SingleAsync();
                }
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<string>> ChangePasswordAsync(ChangePasswordViewModel parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                if (!await Db.Queryable<WorkingPersonnel>().Where(it => it.UserId == Config.userid && it.Password == parm.OldPassword).AnyAsync())
                {
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = "旧密码不正确";
                }
                else
                {
                    await Db.Updateable<WorkingPersonnel>()
                        .SetColumns(it => new WorkingPersonnel() { Password = parm.NewPassword })
                        .Where(it => it.UserId == Config.userid).ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<string>> DeleteAsync(int id)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                await Db.Updateable<WorkingPersonnel>()
                    .SetColumns(it => new WorkingPersonnel() { LoginName = null,UserId = null, IsDel = true })
                    .Where(it => it.Id == id).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public Task<ApiResult<WorkingPersonnel>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<WorkingPersonnel>> GetByOpenIdAsync()
        {
            throw new NotImplementedException();
        }
        

        public async Task<ApiResult<List<WorkingPersonnelViewModel>>> GetListAsync()
        {
            var res = new ApiResult<List<WorkingPersonnelViewModel>>() { statusCode = 200 };
            try
            {
                res.data = await Db.Queryable<WorkingPersonnel,Role>((t1,t2)=>new object[]
                    {
                        JoinType.Inner,t1.RoleId == t2.RoleId
                    })
                    .Where((t1,t2)=>t1.Visible && t1.IsDel == false)
                    .OrderBy((t1, t2) => t1.Id, OrderByType.Desc)
                    .Select<WorkingPersonnelViewModel>().ToListAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<string>> ModifyAsync(WorkingPersonnel parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                await Db.Updateable(parm).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }
    }
}

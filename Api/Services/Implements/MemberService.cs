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
    public class MemberService : DbContext, IMemberService
    {
        public async Task<ApiResult<string>> AddAsync(Member parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                parm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //parm.Password = "888888";
                await Db.Insertable(parm).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("biz_member.MemberNo") != -1)
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "会员编号重复";
                }
                else
                {
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = ApiEnum.Error + ex.Message;
                }
            }
            return res;
        }

        public async Task<ApiResult<string>> AddListAsync(List<Member> list)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                await Db.Insertable(list.ToArray()).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("biz_member.MemberNo") != -1)
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "会员编号重复";
                }
                else
                {
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = ApiEnum.Error + ex.Message;
                }
            }
            return res;
        }

        public async Task<ApiResult<string>> DeleteAsync(int id)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                await Db.Updateable<Member>()
                    .SetColumns(it => new Member() { MemberNo = null,IsDel = true })
                    .Where(it => it.MemberId == id).ExecuteCommandAsync();

                await Db.Updateable<User>()
                    .SetColumns(it => new User() { MemberId = null })
                    .Where(it => it.MemberId == id).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<AccountBalanceViewModel>> GetBalanceByUserIdAsync(int? userId = null)
        {
            var res = new ApiResult<AccountBalanceViewModel>() { statusCode = 200 };
            userId = userId ?? Config.userid;
            try
            {
                res.data = await Db.Queryable<User,Member>((t1,t2)=>new object[]
                    {
                        JoinType.Inner,t1.MemberId == t2.MemberId
                    })
                    .Where((t1, t2) => t1.UserId == userId)
                    .Select<AccountBalanceViewModel>().SingleAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }
        public async Task<ApiResult<AccountBalanceViewModel>> BindMemberCardAsync(BindMemberCardViewModel parm)
        {
            var res = new ApiResult<AccountBalanceViewModel>() { statusCode = 200 };
            try
            {
                if (!await Db.Queryable<Member>().Where(it => it.MemberNo == parm.MemberNo).AnyAsync())
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "卡号不存在，请检查！";
                }
                else if (!await Db.Queryable<Member>().Where(it => it.MemberNo == parm.MemberNo && it.Password == parm.Password).AnyAsync())
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "密码错误，请检查！";
                }
                else
                {
                    var m = await Db.Queryable<Member>().Where(it => it.MemberNo == parm.MemberNo && it.Password == parm.Password).SingleAsync();
                    //插入或更新user，返回实体
                    IUserService iu = new UserService();
                    var user = await Db.Queryable<User>().Where(it => it.OpenId == parm.OpenId).SingleAsync();
                    if (user == null)
                        user = await iu.AddAsync(new User { OpenId = parm.OpenId,MemberId = m.MemberId});
                    else
                        await Db.Updateable<User>()
                            .SetColumns(it => new User() { MemberId = m.MemberId })
                            .Where(it => it.UserId == user.UserId).ExecuteCommandAsync();

                    //返回账户信息
                    res = await GetBalanceByUserIdAsync(user.UserId);
                }
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public Task<ApiResult<Member>> GetByMemberNoAsync(string memberNo)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<Member>> GetByUserIdAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<Page<MemberViewModel>>> GetPageListAsync(MemberPageListParmModel pageParm)
        {
            var res = new ApiResult<Page<MemberViewModel>>();
            RefAsync<int> totalCount = 0;
            var totalPages = 0;
            try
            {
                var list = await Db.Queryable<Member>()
                    .WhereIF(SqlFunc.HasValue(pageParm.QueryWords), it => it.MemberNo.Contains(pageParm.QueryWords) || it.MemberName.Contains(pageParm.QueryWords))
                    .Where(it => it.IsDel == false)
                    .OrderBy(it => it.MemberId, OrderByType.Desc)
                    .Select<MemberViewModel>()
                    .ToPageListAsync(pageParm.CurrentPage, pageParm.Limit, totalCount);

                totalPages = totalCount % pageParm.Limit == 0 ? totalCount / pageParm.Limit : (totalCount / pageParm.Limit) + 1;
                var page = new Page<MemberViewModel>()
                {
                    CurrentPage = pageParm.CurrentPage,
                    TotalPages = totalPages,
                    TotalItems = totalCount,
                    ItemsPerPage = pageParm.Limit,
                    Items = list ?? new List<MemberViewModel>()
                };
                res.message = "获取成功！";
                res.data = page;
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<string>> ModifyAsync(Member parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                //只更新编号、名称、电话
                await Db.Updateable(parm).UpdateColumns(it => new { it.MemberNo, it.MemberName, it.Tel }).ExecuteCommandAsync();
                //await Db.Updateable(parm).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<string>> ResetPasswordAsync(int id)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                await Db.Updateable<Member>()
                    .SetColumns(it => new Member() { Password = "888888" })
                    .Where(it => it.MemberId == id).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<string>> ChangePasswordCusAsync(string newPassword)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                var memberId = await Db.Queryable<User>().Where(it => it.UserId == Config.userid).Select(it => it.MemberId).SingleAsync();

                await Db.Updateable<Member>()
                    .SetColumns(it => new Member() { Password = newPassword })
                    .Where(it => it.MemberId == memberId).ExecuteCommandAsync();
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

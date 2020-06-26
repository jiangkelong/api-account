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
    public class AccountService : DbContext, IAccountService
    {
        public async Task<ApiResult<string>> ChangeBalanceAsync(RechargeViewModel parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                //变动余额
                var changeMoney = parm.Type == "recharge" ? parm.Money : (parm.Money * -1);
                var changeNum = parm.Type == "recharge" ? parm.Num : (parm.Num * -1);
                await Db.Updateable<Member>()
                    .SetColumns(it => new Member() { Balance_Money = it.Balance_Money + changeMoney, Balance_Num = it.Balance_Num + changeNum })
                    .Where(it => it.MemberId == parm.MemberId)
                    .ExecuteCommandAsync();

                //插入记录
                parm.Money = changeMoney;
                parm.Num = changeNum;
                parm.MakeManId = Config.userid;
                InsertAccountLog(parm);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Out of range value for column") != -1)
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "余额不足";
                }
                else
                {
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = ApiEnum.Error + ex.Message;
                }
            }
            return res;
        }

        public async Task<ApiResult<Page<CustomerBillViewModel>>> GetBillPageListAsync(PageParm pageParm)
        {
            var res = new ApiResult<Page<CustomerBillViewModel>>();
            try
            {
                var list = await Db.Queryable<AccountItems, User>((t1, t2) => new object[]
                      {
                        JoinType.Inner,t1.MemberId == t2.MemberId
                      })
                    .Where((t1, t2) => t2.UserId == Config.userid)
                    .OrderBy((t1, t2) => t1.Id, OrderByType.Desc)
                    .Select((t1, t2) => new CustomerBillViewModel
                    {
                        Type = t1.Type,
                        CreatedOn = t1.CreatedOn,
                        Money = t1.Money,
                        Num = t1.Num
                    })
                    .ToPageListAsync(pageParm.CurrentPage, pageParm.Limit);


                var page = new Page<CustomerBillViewModel>()
                {
                    CurrentPage = pageParm.CurrentPage,
                    ItemsPerPage = pageParm.Limit,
                    Items = list ?? new List<CustomerBillViewModel>()
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

        public ApiResult<List<AccountLogViewModel>> GetLogList(AccountLogPageListParmModel pageParm)
        {
            var res = new ApiResult<List<AccountLogViewModel>>();
            pageParm.BeginDate = string.IsNullOrEmpty(pageParm.BeginDate) ? "1991-01-01" : pageParm.BeginDate;
            pageParm.EndDate = string.IsNullOrEmpty(pageParm.EndDate) ? "2050-01-01" : pageParm.EndDate;
            try
            {
                var list = Db.Queryable<AccountItems, Member, WorkingPersonnel>((t1, t2, t3) => new object[]
                      {
                        JoinType.Inner,t1.MemberId == t2.MemberId,
                        JoinType.Inner,t1.MakeManId == t3.UserId
                      })
                    .WhereIF(SqlFunc.HasValue(pageParm.Type) && pageParm.Type != "全部", (t1, t2, t3) => t1.Type.Equals(pageParm.Type))
                    .WhereIF(SqlFunc.HasValue(pageParm.MemberNo), (t1, t2, t3) => t2.MemberNo.Contains(pageParm.MemberNo))
                    .WhereIF(SqlFunc.HasValue(pageParm.MemberName), (t1, t2, t3) => t2.MemberName.Contains(pageParm.MemberName))
                    .WhereIF(SqlFunc.HasValue(pageParm.MakeMan), (t1, t2, t3) => t3.Name.Contains(pageParm.MakeMan))
                    .Where((t1, t2, t3) => SqlFunc.Between(SqlFunc.Substring(t1.CreatedOn, 0, 10), pageParm.BeginDate, pageParm.EndDate))
                    .OrderBy((t1, t2, t3) => t1.Id, OrderByType.Desc)
                    .Select((t1, t2, t3) => new AccountLogViewModel
                    {
                        MemberNo = t2.MemberNo,
                        MemberName = t2.MemberName,
                        Type = t1.Type,
                        MakeMan = t3.Name,
                        CreatedOn = t1.CreatedOn,
                        Money = t1.Money,
                        Num = t1.Num
                    })
                    .ToList();

                res.message = "获取成功！";
                res.data = list;
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            return res;
        }

        public async Task<ApiResult<Page<AccountLogViewModel>>> GetLogPageListAsync(AccountLogPageListParmModel pageParm)
        {
            var res = new ApiResult<Page<AccountLogViewModel>>();
            pageParm.BeginDate = string.IsNullOrEmpty(pageParm.BeginDate) ? "1991-01-01" : pageParm.BeginDate;
            pageParm.EndDate = string.IsNullOrEmpty(pageParm.EndDate) ? "2050-01-01" : pageParm.EndDate;
            RefAsync<int> totalCount = 0;
            var totalPages = 0;
            try
            {
                var roleId = await Db.Queryable<WorkingPersonnel>().Where(it => it.UserId == Config.userid).Select(it => it.RoleId).SingleAsync();
                var list = await Db.Queryable<AccountItems, Member, WorkingPersonnel>((t1, t2, t3) => new object[]
                      {
                        JoinType.Inner,t1.MemberId == t2.MemberId,
                        JoinType.Inner,t1.MakeManId == t3.UserId
                      })
                    .WhereIF(roleId == 2, (t1, t2, t3) => t1.MakeManId == Config.userid)
                    .WhereIF(SqlFunc.HasValue(pageParm.Type) && pageParm.Type != "全部", (t1, t2, t3) => t1.Type.Equals(pageParm.Type))
                    .WhereIF(SqlFunc.HasValue(pageParm.MemberNo), (t1, t2, t3) => t2.MemberNo.Contains(pageParm.MemberNo))
                    .WhereIF(SqlFunc.HasValue(pageParm.MemberName), (t1, t2, t3) => t2.MemberName.Contains(pageParm.MemberName))
                    .WhereIF(SqlFunc.HasValue(pageParm.MakeMan), (t1, t2, t3) => t3.Name.Contains(pageParm.MakeMan))
                    .Where((t1, t2, t3) => SqlFunc.Between(SqlFunc.Substring(t1.CreatedOn, 0, 10), pageParm.BeginDate, pageParm.EndDate))
                    .OrderBy((t1, t2, t3) => t1.Id, OrderByType.Desc)
                    .Select((t1, t2, t3) => new AccountLogViewModel
                    {
                        MemberNo = t2.MemberNo,
                        MemberName = t2.MemberName,
                        Type = t1.Type,
                        MakeMan = t3.Name,
                        CreatedOn = t1.CreatedOn,
                        Money = t1.Money,
                        Num = t1.Num
                    })
                    .ToPageListAsync(pageParm.CurrentPage, pageParm.Limit, totalCount);

                totalPages = totalCount % pageParm.Limit == 0 ? totalCount / pageParm.Limit : (totalCount / pageParm.Limit) + 1;

                var page = new Page<AccountLogViewModel>()
                {
                    CurrentPage = pageParm.CurrentPage,
                    TotalPages = totalPages,
                    TotalItems = totalCount,
                    ItemsPerPage = pageParm.Limit,
                    Items = list ?? new List<AccountLogViewModel>()
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
        public void InsertAccountLog(RechargeViewModel parm)
        {
            var item = new AccountItems
            {
                CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                MemberId = parm.MemberId,
                MakeManId = parm.MakeManId,
                Money = parm.Money,
                Num = parm.Num,
                Type = parm.Type == "recharge" ? "充值" : "收款"
            };
            Db.Insertable(item).ExecuteCommand();
        }

        public async Task<ApiResult<string>> PaySubmitAsync(PaymentViewModel parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                var check = await Db.Queryable<Member, User>((t1, t2) => new object[]
                      {
                        JoinType.Inner,t1.MemberId == t2.MemberId
                      })
                    .Where((t1, t2) => t2.UserId == Config.userid && t1.Password == parm.Password)
                    .AnyAsync();
                if (!check)
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "密码不正确";
                }
                else
                {
                    //变动余额
                    var changeMoney = parm.Money * -1;
                    var changeNum = parm.Num * -1;
                    var memberId = await Db.Queryable<User>().Where(it => it.UserId == Config.userid).Select(it => it.MemberId).SingleAsync();
                    await Db.Updateable<Member>()
                        .SetColumns(it => new Member() { Balance_Money = it.Balance_Money + changeMoney, Balance_Num = it.Balance_Num + changeNum })
                        .Where(it => it.MemberId == memberId)
                        .ExecuteCommandAsync();

                    //插入记录
                    var charging = new RechargeViewModel
                    {
                        Money = changeMoney,
                        Num = changeNum,
                        MakeManId = parm.MakeManId,
                        MemberId = (int)memberId,
                        Type = "charging"
                    };
                    InsertAccountLog(charging);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Out of range value for column") != -1)
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "余额不足";
                }
                else
                {
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = ApiEnum.Error + ex.Message;
                }
            }
            return res;
        }
    }
}

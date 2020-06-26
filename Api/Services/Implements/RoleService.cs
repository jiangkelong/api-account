using Api.Common;
using Api.Models;
using Api.Services.Interfaces;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Implements
{
    public class RoleService : DbContext, IRoleService
    {
        public async Task<ApiResult<List<Role>>> GetListAsync()
        {
            var res = new ApiResult<List<Role>>() { statusCode = 200 };
            try
            {
                res.data = await Db.Queryable<Role>().OrderBy(it=>it.RoleId, OrderByType.Desc).ToListAsync();
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

using Api.Common;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<List<Role>>> GetListAsync();
    }
}

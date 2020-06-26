using Api.Common;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResult<List<Menu>>> GetRoleMenuAsync();
    }
}

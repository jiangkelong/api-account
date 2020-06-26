using Api.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ApiResult<string> ImportNewMemberList(IFormFile file);
        ApiResult<string> BacthRecharge(IFormFile file);
    }
}

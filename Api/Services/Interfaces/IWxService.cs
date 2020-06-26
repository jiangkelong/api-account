using Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IWxService
    {
        string GetExistAccessToken();
        ApiResult<string> CreateQRCode(string page);
    }
}

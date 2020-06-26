using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    [SugarTable("wx_AccessToken")]
    public class AccessToken
    {
        public string access_token { set; get; }
        public string expires { set; get; }
    }
    public class AccessTokenResponse
    {
        public string access_token { set; get; }
        public int expires_in { set; get; }
    }
}

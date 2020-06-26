using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public static class Config
    {
        public static string ConnectionString { get; set; }
        /// <summary>
        /// 为了获取上下文HttpContext
        /// 先在startup.cs ConfigureServices中注册
        /// 再在startup.cs Configure中为该属性指定
        /// 这样在程序中就能获取到HttpContextAccessor，并用来访问HttpContext
        /// </summary>
        public static IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 为了获取虚拟路径
        /// </summary>
        public static IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// 网站地址
        /// </summary>
        public static string serverAddress= "https://lsweapp.jiangkelong.com";
        //public static string serverAddress = "https://localhost:44361";
        /// <summary>
        /// 从请求头中获取userid
        /// </summary>
        public static int userid
        {
            get => int.Parse(_httpContextAccessor.HttpContext.Request.Headers["userid"].FirstOrDefault() ?? "0");
        }
        public static string webRootPath
        {
            get => _hostingEnvironment.WebRootPath;
        }
        /// <summary>
        /// 从请求头中获取openid
        /// </summary>
        //public static string openid
        //{
        //    get=> httpContextAccessor.HttpContext.Request.Headers["openid"].FirstOrDefault();
        //}
    }
}

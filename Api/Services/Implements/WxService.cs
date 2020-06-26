using Api.Common;
using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Implements
{
    public class WxService : DbContext, IWxService
    {
        public string GetExistAccessToken()
        {
            var acc = Db.Queryable<AccessToken>().Single() ?? new AccessToken();
            AccessTokenResponse ar = new AccessTokenResponse();
            string token = acc.access_token;
            string time = acc.expires;

            if (string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(time.ToString()))
            {
                ar = GetAccessToken();
                acc.access_token = ar.access_token;

                DateTime _accessExpires = DateTime.Now.AddSeconds(ar.expires_in - 500);
                acc.expires = _accessExpires.ToString();

                Db.Deleteable<AccessToken>().ExecuteCommandAsync();
                Db.Insertable(acc).ExecuteCommand();
            }
            else if (!string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(time.ToString()))
            {
                DateTime AccessExpires = Convert.ToDateTime(time);

                if (DateTime.Now > AccessExpires)
                {

                    ar = GetAccessToken();
                    acc.access_token = ar.access_token;

                    DateTime _accessExpires = DateTime.Now.AddSeconds(ar.expires_in - 500);
                    acc.expires = _accessExpires.ToString();

                    Db.Deleteable<AccessToken>().ExecuteCommand();
                    Db.Insertable(acc).ExecuteCommand();
                }
            }

            token = acc.access_token;
            return token;
        }
        public AccessTokenResponse GetAccessToken()
        {
            string str_accessToken = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + WxConfig.APPID + "&secret=" + WxConfig.APPSECRET;

            string accessToken = GetJson(str_accessToken);

            AccessTokenResponse getAccessToken = JsonConvert.DeserializeObject<AccessTokenResponse>(accessToken);

            return getAccessToken;

        }
        public string GetJson(string url)
        {
            WebClient wc = new WebClient();

            wc.Credentials = CredentialCache.DefaultCredentials;

            wc.Encoding = Encoding.UTF8;;
            string returnText = wc.DownloadString(url);

            if (returnText.Contains("errcode"))
            {
                //可能出错
            }
            return returnText;
        }

        public ApiResult<string> CreateQRCode(string page)
        {
            var res = new ApiResult<string>() { data = string.Empty, statusCode = 200 };
            try
            {
                //string page = "pages/workbench/collectionQrcode/collectionQrcode";

                string DataJson = string.Empty;
                string access_token = GetExistAccessToken();
                //适用于需要的码数量极多，或仅临时使用的业务场景
                //通过该接口生成的小程序码，永久有效，数量暂无限制。
                string url = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + access_token;
                DataJson = "{";
                DataJson += string.Format("\"scene\":\"{0}\",", Config.userid);//所要传的参数
                DataJson += string.Format("\"width\":\"{0}\",", 200);
                DataJson += string.Format("\"page\":\"{0}\",", page);//扫码所要跳转的地址，根路径前不要填加'/'不能携带参数（参数请放在scene字段里），如果不填写这个字段，默认跳主页面
                DataJson += "\"line_color\":{";
                DataJson += string.Format("\"r\":\"{0}\",", 0);
                DataJson += string.Format("\"g\":\"{0}\",", 0);
                DataJson += string.Format("\"b\":\"{0}\"", 0);
                DataJson += "}";
                //DataJson += string.Format("\"isHyaline\":\"{0}\"", true);
                DataJson += "}";


                //string jsondata = JsonConvert.SerializeObject(DataJson);
                //DataJson的配置见小程序开发文档，B接口：https://mp.weixin.qq.com/debug/wxadoc/dev/api/qrcode.html
                res.data = CreateWeChatQrCode(url, DataJson);
            }
            catch (Exception e)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + e.Message;
            }
            //res.data = Config.serverAddress + "/qrcode/test.jpg";
            return res;//返回图片地址
        }
        public string CreateWeChatQrCode(string url,string parm)
        {
            HttpWebRequest request;
            string imgName = string.Empty;
            string path = string.Empty;
            try
            {
                request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";

                byte[] payload = System.Text.Encoding.UTF8.GetBytes(parm);
                request.ContentLength = payload.Length;

                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(payload, 0, payload.Length);
                    writer.Close();
                }

                System.Net.HttpWebResponse response;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream s;
                s = response.GetResponseStream();//返回图片数据流

                byte[] tt = Utils.StreamToBytes(s);//将数据流转为byte[]

                //在文件名前面加上时间，以防重名
                imgName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
                //文件存储相对于当前应用目录的虚拟目录
                path = Config.webRootPath + "/qrcode/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllBytes(path + imgName,tt);//将byte[]存储为图片
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Config.serverAddress + "/qrcode/" + imgName;
        }
    }
}

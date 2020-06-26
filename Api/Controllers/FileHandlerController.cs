using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Common;
using Api.Models.ViewModels;
using Api.Services.Implements;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileHandlerController : ControllerBase
    {
        //private readonly IHostingEnvironment _hostingEnvironment;
        //public FileHandlerController(IHostingEnvironment hostingEnvironment)
        //{
        //    _hostingEnvironment = hostingEnvironment;
        //}
        IFileService _iFileService = new FileService();
        IAccountService _iAccountService = new AccountService();
        /// <summary>
        /// 批量添加会员
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public IActionResult ImportNewMemberList(IFormCollection files)
        {
            IFormFile file = files.Files[0];
            var res = _iFileService.ImportNewMemberList(file);
            return Ok(res);
        }
        /// <summary>
        /// 批量充值
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public IActionResult BacthRecharge(IFormCollection files)
        {
            IFormFile file = files.Files[0];
            var res = _iFileService.BacthRecharge(file);
            return Ok(res);
        }
        /// <summary>
        /// 导出账目
        /// </summary>
        /// <param name="pageParm"></param>
        /// <returns></returns>
        public IActionResult ExportAccountItems(AccountLogPageListParmModel pageParm)
        {
            string title = "";
            if(!string.IsNullOrEmpty(pageParm.BeginDate)|| !string.IsNullOrEmpty(pageParm.EndDate))
            {
                title = "(" + pageParm.BeginDate + "—" + pageParm.EndDate + ")";
            }
            //获取需要导出的list列表数据
            var responseData = _iAccountService.GetLogList(pageParm);
            if (responseData.statusCode != 200)
            {
                return null;
            }
            #region 生成xls
            List<string> lstTitle = new List<string>()
            {
                "类型","操作人","会员编号","会员名称","金额","数量","时间"
            };
            IWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet("sheet1");
            IRow rowTitle = sheet.CreateRow(0);
            ICellStyle style = book.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;//垂直居中
            for (int i = 0; i < lstTitle.Count; i++)
            {
                rowTitle.CreateCell(i).SetCellValue(lstTitle[i]);
            }
            var list = responseData.data;
            string plus_minus = "";//正负号
            decimal total_money = 0;
            decimal total_num = 0;
            if (list != null)
            {
                int cellIndex = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    plus_minus = list[i].Type == "充值" ? "+" : "";
                    cellIndex = 0;
                    IRow row = sheet.CreateRow(i + 1);
                    row.CreateCell(cellIndex++).SetCellValue(list[i].Type);
                    row.CreateCell(cellIndex++).SetCellValue(list[i].MakeMan);
                    row.CreateCell(cellIndex++).SetCellValue(list[i].MemberNo);
                    row.CreateCell(cellIndex++).SetCellValue(list[i].MemberName);
                    row.CreateCell(cellIndex++).SetCellValue(list[i].Money == 0 ? "" : (plus_minus + list[i].Money.ToString() + "元"));
                    row.CreateCell(cellIndex++).SetCellValue(list[i].Num == 0 ? "" : (plus_minus + list[i].Num.ToString() + "桶"));
                    row.CreateCell(cellIndex++).SetCellValue(list[i].CreatedOn);
                    total_money += (decimal)list[i].Money;
                    total_num += (decimal)list[i].Num;
                }
                //合计
                IRow t_row = sheet.CreateRow(list.Count + 2);
                t_row.CreateCell(0).SetCellValue("合计");
                t_row.CreateCell(4).SetCellValue(total_money.ToString() + "元");
                t_row.CreateCell(5).SetCellValue(total_num.ToString() + "桶");
            }
            #endregion
            for (int i = 0; i < lstTitle.Count; i++)
            {
                sheet.AutoSizeColumn(i);//i：根据标题的个数设置自动列宽
            }

            MemoryStream stream = new MemoryStream();
            book.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(stream, "application/ms-excel;charset=utf-8", "账目流水" + title + ".xls");
        }
    }
}
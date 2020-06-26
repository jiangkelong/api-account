using Api.Common;
using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Implements
{
    public class FileService : DbContext, IFileService
    {
        public ApiResult<string> ImportNewMemberList(IFormFile file)
        {
            var res = new ApiResult<string>();
            try
            {
                if (file == null || file.Length <= 0)
                {
                    res.message = "文件为空！";
                    res.statusCode = (int)ApiEnum.Error;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = ReadStreamToDataTable(file.OpenReadStream(), Path.GetExtension(file.FileName), 3);
                    var insertList = new List<Member>();
                    int insertCount = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //编号或名称为空的不插入
                        if (string.IsNullOrEmpty(dt.Rows[i]["会员编号"].ToString()) || string.IsNullOrEmpty(dt.Rows[i]["会员名称"].ToString()))
                        {
                            continue;
                        }
                        insertList.Add(new Member
                        {
                            MemberNo = dt.Rows[i]["会员编号"].ToString(),
                            MemberName = dt.Rows[i]["会员名称"].ToString(),
                            Tel = dt.Rows[i]["联系方式"].ToString(),
                            CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                        insertCount++;
                    }
                    res.message = insertCount.ToString();
                    //批量插入
                    Db.Insertable(insertList.ToArray()).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("biz_member.MemberNo") != -1)
                {
                    res.statusCode = (int)ApiEnum.CustomError;
                    res.message = "导入失败：会员编号重复";
                }
                else
                {
                    res.message = "导入失败：" + ex.Message;
                    res.statusCode = (int)ApiEnum.Error;
                }
            }
            return res;
        }
        public  ApiResult<string> BacthRecharge(IFormFile file)
        {
            var res = new ApiResult<string>();
            try
            {
                if (file == null || file.Length <= 0)
                {
                    res.message = "文件为空！";
                    res.statusCode = (int)ApiEnum.Error;
                }
                else
                {
                    int userId = Config.userid;
                    DataTable dt = new DataTable();
                    dt = ReadStreamToDataTable(file.OpenReadStream(), Path.GetExtension(file.FileName), 3);
                    var insertList = new List<AccountItems>();
                    int insertCount = 0;
                    decimal money = 0;
                    decimal num = 0;
                    string memberNo = "";
                    int memberId;

                    Db.Ado.BeginTran();//开始事务

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //编号为空的无效
                        if (string.IsNullOrEmpty(dt.Rows[i]["会员编号"].ToString()))
                        {
                            continue;
                        }
                        money = 0;
                        num = 0;
                        decimal.TryParse(dt.Rows[i]["充值金额"].ToString(),out money);
                        decimal.TryParse(dt.Rows[i]["充值数量"].ToString(), out num);
                        memberNo = dt.Rows[i]["会员编号"].ToString();
                        memberId = Db.Queryable<Member>().Where(it => it.MemberNo == memberNo).Select(it => it.MemberId).Single();

                        //更改账户余额
                        Db.Updateable<Member>()
                            .SetColumns(it => new Member() { Balance_Money = it.Balance_Money + money, Balance_Num = it.Balance_Num + num })
                            .Where(it => it.MemberId == memberId).ExecuteCommand();

                        insertList.Add(new AccountItems
                        {
                            CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            MemberId = memberId,
                            MakeManId = userId,
                            Money = money,
                            Num = num,
                            Type = "充值"
                        });
                        insertCount++;
                    }
                    res.message = insertCount.ToString();
                    //批量插入日志
                    Db.Insertable(insertList.ToArray()).ExecuteCommand();

                    Db.Ado.CommitTran();//提交事务
                }
            }
            catch (Exception ex)
            {
                res.message = "导入失败：" + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Db.Ado.RollbackTran();//事务回滚
            }
            return res;
        }
        /// <summary>
        /// 将文件流读取到DataTable数据表中
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="cellCount">总列数</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadStreamToDataTable(Stream fileStream, string fileType, int? cellCount = null, bool isFirstRowColumn = true, string sheetName = null)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (fileType != ".xls" && fileType != ".xlsx")
                {
                    throw new Exception("传入的不是Excel文件！");
                }
                //根据文件流创建excel数据结构,NPOI的工厂类WorkbookFactory会自动识别excel版本，创建出不同的excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    if (cellCount == null)
                        cellCount = firstRow.LastCellNum;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null || row.FirstCellNum < 0) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            //同理，没有数据的单元格都默认是null
                            ICell cell = row.GetCell(j);
                            if (cell != null)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    //判断是否日期类型
                                    if (DateUtil.IsCellDateFormatted(cell))
                                    {
                                        dataRow[j] = row.GetCell(j).DateCellValue;
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).ToString().Trim();
                                    }
                                }
                                else
                                {
                                    dataRow[j] = row.GetCell(j).ToString().Trim();
                                }
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

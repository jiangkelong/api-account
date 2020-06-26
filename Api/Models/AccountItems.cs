using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    /// <summary>
    /// 账目实体类
    /// </summary>
    [SugarTable("biz_account_items")]
    public class AccountItems:BaseModel
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { set; get; }
        public int MemberId { set; get; }
        public int MakeManId { set; get; }
        public decimal? Money { set; get; }
        public decimal? Num { set; get; }
        public string Type { set; get; }
    }
}

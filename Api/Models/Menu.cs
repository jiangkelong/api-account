using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    [SugarTable("sys_menu")]
    public class Menu
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string TreeID { set; get; }
        public string Title { set; get; }
        public int Leaf { set; get; }
        public string Url { set; get; }
        public string ParentID { set; get; }
        public List<Menu> Children { set; get; }
    }
}

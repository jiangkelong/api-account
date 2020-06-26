using Api.Common;
using Api.Models;
using Api.Services.Interfaces;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Implements
{
    public class AdminService : DbContext, IAdminService
    {
        public async Task<ApiResult<List<Menu>>> GetRoleMenuAsync()
        {
            var res = new ApiResult<List<Menu>>
            {
                statusCode = 200
            };
            var list = new List<Menu>();
            var treeList = new List<Menu>();
            try
            {
                var roleId = await Db.Queryable<WorkingPersonnel>().Where(it => it.UserId == Config.userid).Select(it => it.RoleId).SingleAsync();
                if(roleId == 1)
                {
                    //管理员
                    list = Db.Queryable<Menu>().Select(m => new Menu()
                    {
                        TreeID = m.TreeID,
                        Title = m.Title,
                        Url = m.Url,
                        Leaf = m.Leaf,
                        ParentID = m.ParentID
                    }).ToList();
                }
                else
                {
                    list = Db.Queryable<Menu, RoleMenu>((m, r) => new object[]{
                        JoinType.Inner,m.TreeID == r.TreeID
                    }).Where((m, p) => p.RoleID == roleId).Select((m, p) => new Menu()
                    {
                        TreeID = m.TreeID,
                        Title = m.Title,
                        Url = m.Url,
                        Leaf = m.Leaf,
                        ParentID = m.ParentID
                    }).ToList();
                }
                foreach(var item in list.Where(m => m.ParentID == "0").OrderBy(m => m.TreeID))
                {
                    //获得子级
                    var children = TreeChildren(list, new List<Menu>(), item.TreeID);
                    treeList.Add(new Menu()
                    {
                        TreeID = item.TreeID,
                        Title = item.Title,
                        Url = item.Url,
                        Leaf = item.Leaf,
                        ParentID = item.ParentID,
                        Children = children.Count == 0 ? null : children
                    });
                }
                if (treeList.Count() == 0)
                {
                    res.success = false;
                    res.statusCode = (int)ApiEnum.Error;
                    res.message = "无PC端权限";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error + ex.Message;
            }
            res.data = treeList;
            return res;
        }
        /// <summary>
        /// 递归获取子节点
        /// </summary>
        List<Menu> TreeChildren(List<Menu> sourceList, List<Menu> list, string id)
        {
            foreach (var row in sourceList.Where(m => m.ParentID == id).OrderBy(m => m.TreeID))
            {
                var res = TreeChildren(sourceList, new List<Menu>(), row.TreeID);
                list.Add(new Menu()
                {
                    TreeID = row.TreeID,
                    Title = row.Title,
                    Url = row.Url,
                    Leaf = row.Leaf,
                    ParentID = row.ParentID,
                    Children = res.Count > 0 ? res : null
                });
            }
            return list;
        }
    }
}

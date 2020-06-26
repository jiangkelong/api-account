using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public class DbContext
    {
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                //可以在连接字符串中设置连接池pooling=true;表示开启连接池
                //eg:min pool size=2;max poll size=4;表示最小连接池为2，最大连接池是4；默认是100
                //ConnectionString = "database='" + "mysql" + "';Data Source = '" + "127.0.0.1" + "'; User Id = '" + "root" + "'; pwd='" + "root" + "';charset='utf8';pooling=true",
                ConnectionString = Config.ConnectionString,
                DbType = SqlSugar.DbType.MySql,//我这里使用的是Mysql数据库
                IsAutoCloseConnection = true,//自动关闭连接
                InitKeyType = InitKeyType.Attribute//从特性读取主键和自增列信息
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //string s = sql;
                Console.WriteLine(sql + "\r\n" +
                    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
        }
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<DbModel> GetDb<DbModel>() where DbModel : class, new()
        {
            return new SimpleClient<DbModel>(Db);
        }
    }
}

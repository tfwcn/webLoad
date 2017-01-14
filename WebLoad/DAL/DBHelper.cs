using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace WebLoad.DAL
{
    public abstract class DBHelper
    {
        protected string strConn = "";
        public DBHelper(string strConn)
        {
            this.strConn = strConn;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="strSql">插入语句</param>
        /// <param name="dt">数据，列名映射成参数</param>
        public abstract void RunSql(string strSql, DataTable dt);
        protected virtual DbType GetDbType(Type t)
        {
            DbType dbType;
            switch (t.Name.ToLower())
            {
                case "int32":
                    dbType = DbType.Int32;
                    break;
                case "Int64":
                    dbType = DbType.Int64;
                    break;
                case "string":
                    dbType = DbType.String;
                    break;
                case "boolean":
                    dbType = DbType.Boolean;
                    break;
                case "datetime":
                    dbType = DbType.DateTime;
                    break;
                case "decimal":
                    dbType = DbType.Decimal;
                    break;
                case "float":
                    dbType = DbType.Decimal;
                    break;
                default:
                    if (t.IsEnum)
                    {
                        dbType = DbType.Int32;
                        break;
                    }
                    throw new Exception("DbType轉換,未定義類型：" + t.Name);
            }
            return dbType;
        }
    }
}

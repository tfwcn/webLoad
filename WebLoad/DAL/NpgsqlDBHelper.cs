using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace WebLoad.DAL
{
    public class NpgsqlDBHelper : DBHelper
    {
        public NpgsqlDBHelper(string strConn)
            : base(strConn)
        {
            this.strConn = strConn;
        }
        private NpgsqlConnection GetConn()
        {
            return new NpgsqlConnection(strConn);
        }

        public override void RunSql(string strSql, DataTable dt)
        {
            using (NpgsqlConnection conn = GetConn())
            {
                conn.Open();
                using (NpgsqlTransaction t = conn.BeginTransaction())
                {
                    using (NpgsqlCommand com = new NpgsqlCommand(strSql, conn, t))
                    {
                        using (NpgsqlDataAdapter da = new NpgsqlDataAdapter())
                        {
                            da.InsertCommand = com;
                            com.CommandText = strSql;
                            com.Parameters.Add(new NpgsqlParameter() { ParameterName = "@uuid", DbType = DbType.AnsiStringFixedLength, Size = 36, Value = Guid.NewGuid().ToString() });
                            foreach (DataColumn col in dt.Columns)
                            {
                                com.Parameters.Add(new NpgsqlParameter() { ParameterName = "@" + col.ColumnName, DbType = GetDbType(col.DataType), SourceColumn = col.ColumnName });
                            }
                            da.InsertCommand.UpdatedRowSource = UpdateRowSource.None;//批量更新必须
                            da.UpdateBatchSize = 0;//批量更新最大值
                            dt.AcceptChanges();
                            foreach (DataRow row in dt.Rows)
                            {
                                row.SetAdded();
                            }
                            da.Update(dt);
                        }
                    }
                    t.Commit();
                }
            }
        }
    }
}

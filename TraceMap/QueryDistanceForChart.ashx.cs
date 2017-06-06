using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using System.Web;

namespace TraceMap
{
    /// <summary>
    /// QueryDistanceForChart 的摘要说明
    /// </summary>
    public class QueryDistanceForChart : QueryTraceData, IHttpHandler
    {
        override protected string QueryTrace(HttpContext context, string result)
        {
            string year = context.Request.QueryString["year"];
            string month = context.Request.QueryString["month"];
            string day = context.Request.QueryString["day"];

            string username = context.Request.QueryString["username"];
            int id = Convert.ToInt32(username);

            //建立数据库的连接
            NpgsqlConnection connection = new NpgsqlConnection("Server = 139.196.243.189;Port = 5432;UserId = postgres;" +
                "Password = postgres;Database = TraceMap");
            connection.Open();

            //构建SQL语句进行查询
            for (int i = 0; i < 24; i += 2)
            {
                TimeStamp timeStamp1 = new TimeStamp(year, month, day, i.ToString());
                TimeStamp timeStamp2;
                if (i + 2 == 24)
                {
                    timeStamp2 = new TimeStamp(year, month, day + 2, 0.ToString());
                }
                else
                {
                    timeStamp2 = new TimeStamp(year, month, day, (i + 2).ToString());
                }
                string sqlCommand = String.Format("SELECT lat,lon FROM trace WHERE person = {0} AND t > '{1}' AND t < '{2}' ORDER BY t", id, timeStamp1.GetTime, timeStamp2.GetTime);
                NpgsqlCommand command = new NpgsqlCommand(sqlCommand, connection);
                NpgsqlDataReader reader = command.ExecuteReader();

                //获取数据
                while (reader.Read())
                {
                    double lat = reader.GetDouble(0);
                    double lon = reader.GetDouble(1);
                    BPoint pt = new BPoint(lat, lon);
                    result += pt.GetData;
                }
                result += "|";
                reader.Close();
            }

            return result;
        }
    }
}
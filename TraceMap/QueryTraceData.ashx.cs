using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;

namespace TraceMap
{
    /// <summary>
    /// QueryTraceData 的摘要说明
    /// </summary>
    public class QueryTraceData : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = "";

            result = QueryTrace(context, result);

            context.Response.Write(result);
        }

        virtual protected string QueryTrace(HttpContext context, string result)
        {
            string username = context.Request.QueryString["username"];
            int id = Convert.ToInt32(username);

            //建立数据库的连接
            NpgsqlConnection connection = new NpgsqlConnection("Server = 139.196.243.189;Port = 5432;UserId = postgres;" +
                "Password = postgres;Database = TraceMap");
            connection.Open();

            //构建SQL语句进行查询
            string sqlCommand = "SELECT lat,lon FROM trace WHERE person = " + id + " ORDER BY t";
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

            return result;
        }

        public class BPoint
        {
            double lat, lon;
            public BPoint(double lat, double lon)
            {
                this.lat = lat; this.lon = lon;
            }
            public string GetData { get { return lat.ToString() + " " + lon.ToString() + "\n"; } }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
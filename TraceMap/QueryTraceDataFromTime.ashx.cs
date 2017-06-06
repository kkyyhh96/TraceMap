using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;

namespace TraceMap
{
    /// <summary>
    /// QueryTraceDataFromTime 的摘要说明
    /// </summary>
    public class QueryTraceDataFromTime : QueryTraceData, IHttpHandler
    {
        override protected string QueryTrace(HttpContext context, string result)
        {
            string start_time_Y = context.Request.QueryString["sY"];
            string start_time_M = context.Request.QueryString["sM"];
            string start_time_D = context.Request.QueryString["sD"];
            string start_time_H = context.Request.QueryString["sH"];
            string start_time = new TimeStamp(start_time_Y, start_time_M, start_time_D, start_time_H).GetTime;

            string end_time_Y = context.Request.QueryString["eY"];
            string end_time_M = context.Request.QueryString["eM"];
            string end_time_D = context.Request.QueryString["eD"];
            string end_time_H = context.Request.QueryString["eH"];
            string end_time = new TimeStamp(end_time_Y, end_time_M, end_time_D, end_time_H).GetTime;

            string username = context.Request.QueryString["username"];
            int id = Convert.ToInt32(username);

            //建立数据库的连接
            NpgsqlConnection connection = new NpgsqlConnection("Server = 139.196.243.189;Port = 5432;UserId = postgres;" +
                "Password = postgres;Database = TraceMap");
            connection.Open();

            //构建SQL语句进行查询
            string sqlCommand = String.Format("SELECT lat,lon FROM trace WHERE person = {0} AND t > '{1}' AND t < '{2}' ORDER BY t", id, start_time, end_time);
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
    }
}
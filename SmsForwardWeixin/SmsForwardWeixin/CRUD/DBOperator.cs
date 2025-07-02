using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;

namespace SmsForwardWeixin.CRUD
{
    public class DBOperator
    {
        string dbfile = @"Data Source=data.db";
        SqliteConnection cnn;
        public DBOperator()
        {
            cnn = new SqliteConnection();
            cnn.ConnectionString = dbfile;
            cnn.Open();
        }

        public string CheckAppointmentReady(string day, string wxid, string platform)   // 检查申请是否就绪（是否允许申请）
        {
            int maxnum = 0;
            int endtime = 0;
            string sqlOperator = "";
            string dateString = DateTime.Now.ToString("yyyyMMdd");
            int now = Convert.ToInt32(dateString);  // 今天日期（20250715）
            int nowtime = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));

            sqlOperator = $"Select Count(*) From config Where time = \'{day}\'";
            var dr = new SqliteCommand(sqlOperator, cnn).ExecuteReader();
            dr.Read();

            if (dr.GetInt32(0) != 1)
            {
                return "预约失败：当天未开放预约，不允许预约。您可以预约其他日期。如需查看可以预约的天数及预约人数，请回复\"查询预约状态\"";
            }

            sqlOperator = $"Select * From config Where time = \'{day}\'";   // 读取配置
            dr = new SqliteCommand(sqlOperator, cnn).ExecuteReader();
            dr.Read();
            maxnum = dr.GetInt32(3);    //顺便将最大预约人数读取了。
            endtime = dr.GetInt32(5);

            sqlOperator = $"Select Count(*) From appointment Where wxid = \'{wxid}\'";  // 检查是否已经存在申请
            dr = new SqliteCommand(sqlOperator, cnn).ExecuteReader();
            dr.Read();
            if (dr.GetInt32(0) >= 1 && platform != "manager") {
                sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
                dr = new SqliteCommand(sqlOperator, cnn).ExecuteReader();
                dr.Read();
                int app_time = dr.GetInt32(3);
                if (app_time == now) return $"您已预约当天号码。预约日期是{app_time}";
                return $"预约失败：您已在{app_time}当天存在一个有效的预约。如需取消预约或更换预约天数，请先回复\"取消预约\"。";
            }
            if (now < maxnum) return "预约失败：当日工作已结束";    // 过时预约检查

            // 对于manager管理员平台，该平台不会检查当日是否存在有效预约或预约时间是否已过。

            if (now == maxnum && platform != "manager")  // 如果在当天预约当天的号码
                if (nowtime >= endtime)    // 如果在当日工作结束后预约
                    return "预约失败：当日可预约时间已过";  // 工作结束后预约

            sqlOperator = $"Select Count(*) From appointment Where time = \'{day}\'";
            dr = new SqliteCommand(sqlOperator, cnn).ExecuteReader();
            dr.Read();
            var currentNum = dr.GetInt32(0);
            if (currentNum > maxnum)
            {
                return $"预约失败：{day}当日最大预约人数为{maxnum}，当前已预约人数为{currentNum}，当前预约人数已满。" +
                    $"您可以预约其他日期。如需查看预约人数，请回复“查询预约人数\"";
            }
            return "OK";
        }
        public string GetInfoByWxid(string wxid)
        {
            // string wxid = "wxid_222333";
            string sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
            SqliteCommand cmd = new SqliteCommand(sqlOperator, cnn);
            SqliteDataReader dr = cmd.ExecuteReader();
            string result = "null";
            while (dr.Read())
            {
                result =  dr.GetString(2);
               
            }
            return result;
        }
        public List<string[]> GetAppointmentListByDay(string day)
        {
            string sqlOperator = $"Select * From appointment Where time = \'{day}\'";
            if (day == "all") sqlOperator = $"Select * From appointment";
            SqliteCommand cmd = new SqliteCommand(sqlOperator, cnn);
            SqliteDataReader dr = cmd.ExecuteReader();
            List<string[]> list = new List<string[]>();
            string[] result = new string[4];
            while (dr.Read())
            {
                result[0] = dr.GetString(1);    // wxid
                result[1] = dr.GetString(3);    // 申请日期
                result[2] = dr.GetString(4);    // 申请时间
                result[3] = dr.GetString(5);    // 当前状态
                list.Add(result);

            }
            return list;
        }
        public string SetAppointmentListByWxid(string wxid, string day,string p)    // p为platform
        {
            if (CheckAppointmentReady(day, wxid,p) is var checkResult && checkResult != "ok") return checkResult; // 检测更改是否符合要求
            string sqlOperator = $"Update appointment Set time = \'{day}\' Where wxid = \'{wxid}\'";
            new SqliteCommand(sqlOperator, cnn).ExecuteReader();
            sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
            SqliteDataReader dr = new SqliteCommand(sqlOperator, cnn).ExecuteReader();
            bool result = false;
            while (dr.Read())
            {
                if (dr.GetString(3) == day) result = true;
            }
            return "OK";
        }
    }

    
}

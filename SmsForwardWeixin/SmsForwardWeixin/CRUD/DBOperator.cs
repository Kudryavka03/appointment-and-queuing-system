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
        public DBOperator()
        {
        }

        public SqliteConnection ConnectDb() {
            SqliteConnection cnn = new SqliteConnection(dbfile);
            cnn.Open();
            return cnn;
        }

        public DataTable RunSqliteCommand(string sql) {

            try
            {
                SqliteConnection cnn = new SqliteConnection(dbfile);
                cnn.Open();
                SqliteDataReader dr = new SqliteCommand(sql, cnn).ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                return dt;
            }
            catch (Exception e)
            {

                return null;
            }
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
            var dr =RunSqliteCommand(sqlOperator);
            if (dr.Rows[0]["Count(*)"].ToString() == "0")  
            {
                return "预约失败：当天未开放预约，不允许预约。您可以预约其他日期。如需查看可以预约的天数及预约人数，请回复\"查询预约状态\"";
            }

            sqlOperator = $"Select * From config Where time = \'{day}\'";   // 读取配置
            dr = RunSqliteCommand(sqlOperator); ;
            maxnum = Convert.ToInt32(dr.Rows[0]["maxnum"].ToString());    //顺便将最大预约人数读取了。
            endtime = Convert.ToInt32(dr.Rows[0]["end_time"].ToString());

            sqlOperator = $"Select Count(*) From appointment Where wxid = \'{wxid}\'";  // 检查是否已经存在申请
            dr = RunSqliteCommand(sqlOperator);
            if (Convert.ToInt32(dr.Rows[0]["Count(*)"].ToString()) >= 1 && platform != "manager")
            {
                sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
                dr = RunSqliteCommand(sqlOperator);
                
                int app_days = Convert.ToInt32(dr.Rows[0]["time"].ToString());
                if (app_days == now) return $"您已预约当天号码。预约日期是{app_days}";
                return $"预约失败：您已在{app_days}当天存在一个有效的预约。如需取消预约或更换预约天数，请先回复\"取消预约\"。";
            }
            if (now < maxnum)
            {
                return "预约失败：当日工作已结束";
            }   // 过时预约检查

            // 对于manager管理员平台，该平台不会检查当日是否存在有效预约或预约时间是否已过。

            if (now == maxnum && platform != "manager")  // 如果在当天预约当天的号码
                if (nowtime >= endtime)
                {    // 如果在当日工作结束后预约
                    return "预约失败：当日可预约时间已过";  // 工作结束后预约
                }

            sqlOperator = $"Select Count(*) From appointment Where time = \'{day}\'";
            dr = RunSqliteCommand(sqlOperator);
            var currentNum = Convert.ToInt32(dr.Rows[0]["Count(*)"].ToString());
            if (currentNum > maxnum)
            {
                return $"预约失败：{day}当日最大预约人数为{maxnum}，当前已预约人数为{currentNum}，当前预约人数已满。" +
                    $"您可以预约其他日期。如需查看预约人数，请回复“查询预约人数\"";
            }
            return "OK";
        }


        public string GetInfoByWxid(string wxid)
        {
            var cnn = ConnectDb();
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
            var cnn = ConnectDb();
            string sqlOperator = $"Select * From appointment Where time = \'{day}\'";
            if (day == "all") sqlOperator = $"Select * From appointment";
            DataTable dr =RunSqliteCommand(sqlOperator);
            List<string[]> list = new List<string[]>();
            string[] result = new string[5];
            result[0] = dr.Rows[0]["wxid"].ToString();    // wxid
            result[5] = dr.Rows[0]["token"].ToString();    // token
            result[1] = dr.Rows[0]["time"].ToString();    // 申请日期
            result[2] = dr.Rows[0]["app_time"].ToString();    // 申请时间
            result[3] = dr.Rows[0]["status"].ToString();    // 当前状态
            list.Add(result);
            return list;
        }
        public string SetAppointmentListByWxid(string day, string wxid, string p)    // p为platform
        {
            if (CheckAppointmentReady(day, wxid,p) is var checkResult && checkResult != "OK") return checkResult; // 检测更改是否符合要求
            string sqlOperator = $"PRAGMA journal_mode=WAL;Update appointment Set time = \'{day}\' Where wxid = \'{wxid}\'";
            RunSqliteCommand(sqlOperator);
            sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
            DataTable dr = RunSqliteCommand(sqlOperator);
            bool result = false;
            if (dr.Rows[0]["time"].ToString() == day) result = true;
            if (result == true) return "OK";
            return "更改失败，系统繁忙。";
        }

        public string AddApointmentFromWxid(string day, string wxid,  string p)
        {
            if (CheckAppointmentReady(day, wxid, p) is var checkResult && checkResult != "OK") return checkResult; // 检测是否符合要求
            var Token = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
            Token = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Token));
            string sqlOperator = $"INSERT INTO appointment (wxid,token,time,app_time,status) VALUES (\'{wxid}\',\'{Token}\',\'{day}\',\'{Token}\',\"PENDING\")";
            RunSqliteCommand(sqlOperator);
            sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
            DataTable dr = RunSqliteCommand(sqlOperator);
            string time = dr.Rows[0]["time"].ToString();
            string key = dr.Rows[0]["token"].ToString();
            if (time !="" || time != null)  // 检测是否已经添加成功
            {
                return $"预约{time}当日成功！您的预约码为{key}。请于当日前往xxxxx地点进行业务办理。" +
                    $"如需更换日期或取消预约，请先发送\"取消预约\"";
            }
            return "预约失败：系统繁忙，请稍后再尝试。";
        }

    }

    
}

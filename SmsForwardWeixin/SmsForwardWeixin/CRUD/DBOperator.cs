using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace SmsForwardWeixin.CRUD
{
    public class DBOperator
    {
        string dbfile = @"Data Source=data.db";
        HttpClient httpClient = null;
        HttpClientHandler handler = new HttpClientHandler();
        
        public DBOperator()
        {
            httpClient = new HttpClient(handler);
            handler.UseCookies = true;
            handler.CookieContainer = new System.Net.CookieContainer();
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

        public string CheckAppointmentReady(string day, string wxid, string platform)   // 检查申请是否就绪（是否允许申请）管理员身份检查将不会检查是否存在有效预约日或时间
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
                return "预约失败：当天未开放预约，不允许预约。您可以预约其他日期。如需查看可以预约的天数及预约人数，请回复\"查询预约计划\"";
            }

            sqlOperator = $"Select * From config Where time = \'{day}\'";   // 读取配置
            dr = RunSqliteCommand(sqlOperator);
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

            // 对于manager管理员平台，该平台不会检查当日是否存在有效预约或预约时间是否已过。但其实可以直接弄一个匿名叫号更加好一点。

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
                    $"您可以预约其他日期。如需查看预约人数，请回复“查询预约计划\"";
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
        public string SetAppointmentListByWxid(string day, string wxid, string p)    // p为platform。这个为直接修改，主要是给管理员后台使用的
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

        public string AddApointmentFromWxid(string day, string wxid,  string p)     // 这个是添加，添加一个新的预约。分两个平台，管理员跟用户端。
        {
            if (CheckAppointmentReady(day, wxid, p) is var checkResult && checkResult != "OK") return checkResult; // 检测是否符合要求
            var Token = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
            Token = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Token));
            string imgPath = Utils.Utils.GenQrCode(Token);
            string sqlOperator = $"INSERT INTO appointment (wxid,token,time,app_time,status) VALUES (\'{wxid}\',\'{Token}\',\'{day}\',\'{Token}\',\'CodeImg Path:{imgPath}\nPENDING\')";
            RunSqliteCommand(sqlOperator);
            sqlOperator = $"Select * From appointment Where wxid = \'{wxid}\'";
            DataTable dr = RunSqliteCommand(sqlOperator);
            string time = dr.Rows[0]["time"].ToString();
            string key = dr.Rows[0]["token"].ToString();
            
            if (time !="" || time != null)  // 检测是否已经添加成功
            {
                if (p == "manager") return $"{Program.imgPath}";
                return $"预约{time}当日成功！请于当日前往xxxxx地点出示现场预约码进行业务办理。" +"请点击以下链接访问您的现场预约码："+ $"{Program.resUrl+"/"+ imgPath}" + 
                    $" 如需更换日期或取消预约，请先发送\"取消预约\"";
            }
            return "预约失败：系统繁忙，请稍后再尝试。";
        }

        public string DelApointmentByWxid(string day, string wxid, string p)    // 取消预约
        {
            int dateString = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            string sqlOperator = $"Select Count(*) From appointment Where wxid = \'{wxid}\'";  // 先检查再预约表中是否存在
            DataTable dr = RunSqliteCommand(sqlOperator);
            var appointment_count = Convert.ToInt32(dr.Rows[0]["Count(*)"].ToString());
            if (appointment_count != 0)   // 存在预约
            {
                int nowtime = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                var appointmentDetailDr = RunSqliteCommand("Select * From appointment Where wxid = \'{wxid}\'");
                var preapp_time = Convert.ToInt32(appointmentDetailDr.Rows[0]["time"].ToString()) ;
                
                var configDr = RunSqliteCommand($"Select * From config Where time = \'{preapp_time}\'");   // 读取天数配置

                var endTime_config = Convert.ToInt32(configDr.Rows[0]["end_time"].ToString());
                if (preapp_time < dateString)
                {
                    return "您没有一个有效的预约，请先预约";
                }
                if (preapp_time == dateString && nowtime >= endTime_config)
                {
                    return $"已为您取消今天的预约。今天的预约时间已结束，您无法继续预约今天的号码，如需继续预约，请预约其他日期的号码。回复\"查询预约计划\"可以查询预约放号计划";
                }
            }
            if (appointment_count == 0)
            {
                return "您没有一个有效的预约，请先预约";
            }
            return "取消预约失败：系统繁忙，请稍后再尝试。";
        }

        public string AppointmentLiveExec(string wxid) // 申请人现场报道
        {
            int nowtime = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));

            string sqlOperator = $"Select Count(*) From appointment Where wxid = \'{wxid}\'"; // 先检查是否存在预约
            var dr = RunSqliteCommand(sqlOperator);
            if (dr.Rows[0]["Count(*)"].ToString() == "0")
            {
                return "现场报到失败：您还没进行预约，请先预约，如需查看操作指引，请回复\"帮助\"";
            }
            dr = RunSqliteCommand($"Select * From appointment Where wxid = \'{wxid}\'");
            var app_days = Convert.ToInt32(dr.Rows[0]["time"].ToString());
            var token = dr.Rows[0]["token"].ToString();
            if (nowtime == app_days)    // 当日预约，符合预约条件
            {
                // 与叫号系统联动，实现叫号。
                // DBOperator中如果出现访问延迟，对于其他用户而言体验是非常不好的。因此不能将HTTP相关内容放到这里执行。我们可以新建一个线程专门实现叫号。
                // 我们可以使用一个简单的队列系统，将额外延迟的东西置于队列中执行。
                Program.callOrder.AddTokenOrder(token);
            }
            else
            {
                return $"现场报道失败：您预约的日期{app_days}并非今天{nowtime}，请于{app_days}当天过来预约";
            }
            // 这里就不作现场报道时间限制了，超时了也肯定报道不了了。
            return "报到失败：系统繁忙，请稍后再试";
        }

        public string AddOrder(string token, string order) 
        {
            string sqlOperator = $"INSERT INTO token_order (token,order) VALUES (\'{token}\',\'{order}\')"; // 添加预约
            var status = RunSqliteCommand(sqlOperator);
            if (status == null)
            {
                return $"预约系统：系统繁忙，为Token:[{token}]添加叫号记录 [号码：{order}] 失败！用户无法自助查看叫号记录";
            }
            return $"预约系统：添加叫号记录 [号码：{order}] 成功！";
        }
    }
}

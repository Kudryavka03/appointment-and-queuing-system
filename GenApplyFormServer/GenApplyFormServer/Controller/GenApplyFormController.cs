using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Routing.Template;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace GenApplyFormServer.Controller
{
    public class GenApplyFormController : ControllerBase
    {
        [HttpPost("genApplyForm")]
        public async Task<IActionResult> Index()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            string request = await reader.ReadToEndAsync();

            int familyTotalNum = 0;
            // string 
            // return BadRequest(question);
            try
            {
                Root Obj = JsonConvert.DeserializeObject<Root>(request);
                Program.Log($"Receive Generate Request From:{Obj.id_number}");
                if (!(Obj.college_name.Contains("大学") || Obj.college_name.Contains("学院")))
                {
                    return BadRequest("学校信息填的是你通过统招（春/夏季高考，三二分段升学，专插本，本升研/博）被录取的大学（高校）名字，你当前填的学校名字不符合要求。");
                }
                if(Obj.deparment_name.Replace(" ","") == "" || Obj.grade.Replace(" ", "") == "" || Obj.major_name.Replace(" ", "") == "")
                {
                    return BadRequest("学校信息填写不完整，请继续补全信息（可以不填写班级，年级不知道就填写入学年份+级，如2025年入学填2025级，不要照抄，结合实际情况填写");
                }
                if (Obj.stu_phone_number.Replace(" ","") == Obj.parent_phone_number.Replace(" ", ""))
                {
                    return BadRequest("家长联系电话不可以与学生相同。如果学生没有手机号码，请先注册号码！");
                }
                if (Obj.stu_phone_number.Length != 11 || Obj.parent_phone_number.Length != 11)
                {
                    return BadRequest("家长或学生手机号码填写不正确");
                }
                if (!Program.TestCardId(Obj.id_number))
                {
                    return BadRequest("身份证校验失败，请重新填写");
                }
                var familyMemberNum = Convert.ToInt32(Obj.family_num);
                var requireFamilyMinNum = familyMemberNum / 2;
                var currentFamilyNum = Obj.family_members.Count;    // 所填写家庭成员信息栏总数
                if (requireFamilyMinNum > currentFamilyNum)
                {
                    return BadRequest($"根据您当前所填写的家庭人口，您至少需要填写{requireFamilyMinNum}个家庭成员信息，您当前填写了{currentFamilyNum}个家庭成员的信息，请继续新增填写。");
                }
                else if (currentFamilyNum < 1)
                {
                    return BadRequest($"家庭人口至少1个人以上，请修改。");
                }
                else if (currentFamilyNum > familyMemberNum)
                {
                    return BadRequest($"您在基本信息一栏中所填写的家庭人口数量为{familyMemberNum}人，但您添加了{currentFamilyNum}个家庭成员信息，不符合常理，请返回修改。");
                }
                int avg_ai = Convert.ToInt32(Obj.avg_ai);
                if (avg_ai > 9999)
                {
                    return BadRequest($"在影响家庭经济困难状况有关信息一栏中，您所填写的家庭人均年收入大于10000（一万）元，请根据家庭实际情况，灵活填写一个小于一万元的数字");
                }
                List<string> familyMemberName = new List<string>();
                int warningJobs = 0;
                List<string> warningJobStr = new List<string>();
                warningJobStr.Add("教师");
                warningJobStr.Add("烟草");
                warningJobStr.Add("卷烟");
                warningJobStr.Add("教育局");
                warningJobStr.Add("教育厅");
                warningJobStr.Add("财政");
                warningJobStr.Add("政府");
                warningJobStr.Add("服务中心");
                warningJobStr.Add("水务");
                warningJobStr.Add("交通局");
                warningJobStr.Add("交通运输");
                warningJobStr.Add("农业农村");
                warningJobStr.Add("卫生局");
                warningJobStr.Add("发展和改革");
                warningJobStr.Add("发改");
                warningJobStr.Add("委员会");
                warningJobStr.Add("公路局");
                warningJobStr.Add("审计");
                warningJobStr.Add("服务大厅");
                warningJobStr.Add("体育局");
                warningJobStr.Add("文化和旅游");
                warningJobStr.Add("政务");
                warningJobStr.Add("检察");
                warningJobStr.Add("人社");
                warningJobStr.Add("人力资源");
                warningJobStr.Add("社会保障");
                warningJobStr.Add("生态环境局");
                warningJobStr.Add("住房");
                warningJobStr.Add("城乡建设");
                warningJobStr.Add("城建局");
                warningJobStr.Add("小学");
                warningJobStr.Add("中学");
                warningJobStr.Add("一中");
                warningJobStr.Add("二中");
                warningJobStr.Add("三中");
                warningJobStr.Add("四中");
                warningJobStr.Add("五中");
                warningJobStr.Add("六中");
                warningJobStr.Add("七中");
                warningJobStr.Add("八中");
                warningJobStr.Add("九中");
                warningJobStr.Add("十中");
                warningJobStr.Add("一小");
                warningJobStr.Add("二小");
                warningJobStr.Add("三小");
                warningJobStr.Add("四小");
                warningJobStr.Add("五小");
                warningJobStr.Add("六小");
                warningJobStr.Add("七小");
                warningJobStr.Add("八小");
                warningJobStr.Add("九小");
                warningJobStr.Add("十小");
                int totalAni = 0;
                foreach (var familyMember in Obj.family_members)
                {
                    string subFamilyMemberName = familyMember.Name;
                    if (familyMemberName.Contains(subFamilyMemberName))
                    {
                        return BadRequest($"您添加了相同姓名的家庭成员，请确认是否有重复");
                    }
                    familyMemberName.Add(familyMember.Name.Replace(" ",""));
                    bool isWarning = false;
                    if (warningJobStr.Contains(familyMember.Pro))
                    {
                        warningJobs++;
                        isWarning = true;
                    }
                    else if (warningJobStr.Contains(familyMember.Workplace))
                    {
                        warningJobs++;
                        isWarning = true;
                    }
                    if (!isWarning)
                    {
                        foreach (var a in warningJobStr)
                        {
                            if (familyMember.Pro.Contains(a))
                            {
                                warningJobs++;
                                break;

                            }
                            else if (familyMember.Workplace.Contains(a))
                            {
                                warningJobs++;
                                break;
                            }
                            
                        }
                    }
                    if(warningJobs > 1 )
                    {
                        return BadRequest($"您添加了两个或以上领财政工资的家庭成员，请适当删除成员或灵活修改地点或职业，如 无固定工作地点 自由职业等等");
                    }
                    if (familyMember.Pro.Replace(" ","") == "" || familyMember.Health.Replace(" ", "") == "" || familyMember.Name.Replace(" ", "") == "" || familyMember.Age.Replace(" ", "") == "" || familyMember.Health.Replace(" ", "") == "")
                    {
                        return BadRequest("请检查你的家庭成员情况，里边任何一个空均不可留空");
                    }
                    try
                    {
                        int i = Convert.ToInt32(familyMember.Ai);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"家庭成员：{familyMember.Name} 年收入填写错误，请重新填写");
                    }
                    totalAni += Convert.ToInt32(familyMember.Ai);
                }
                if ((Math.Abs((totalAni / familyMemberNum) - avg_ai)) > 20000 ) {
                    if (Obj.s1.Replace(" ", "") == " " && Obj.s2.Replace(" ", "") == " " && Obj.s3.Replace(" ", "") == " " && Obj.s4.Replace(" ", "") == " " && Obj.s5.Replace(" ", "") == " ")
                    {
                        return BadRequest($"您所填写的所有家庭成员人均年收入 总数为{totalAni}，但是您在影响家庭经济困难状况有关信息一栏中填写的家庭人均年收入为{avg_ai}，且没有填写任何影响家庭经济困难状况的有关信息，存在较为严重的偏差，请重新灵活填写，控制偏差在20000元内");
                    }

                }
                else if ((Math.Abs((totalAni / familyMemberNum) - avg_ai)) > 2000)
                {
                    return BadRequest($"您所填写的所有家庭成员加起来除以您所填写的家庭成员人数，计算得出人均年收入为{totalAni / familyMemberNum}，但是您在影响家庭经济困难状况有关信息一栏中填写的家庭人均年收入为{avg_ai}，存在较为严重的偏差，请重新填写，控制在2000元偏差内");
                }
                // 检查家庭通讯信息
                if (!(Obj.address.Contains("省") && (Obj.address.Contains("市"))))
                {
                    return BadRequest("家庭通讯信息一栏中的详细通讯地址，填写格式为XX省XX市XXXXX，请按照格式填写");
                }
                try
                {
                    Convert.ToInt32(Obj.postcode);
                    if (Obj.postcode == "" || Obj.postcode.Length != 6)
                    {
                        return BadRequest("邮政编码填写错误。如果不知道，且通讯地址在信宜市请填写525300");
                    }
                }
                catch
                {
                    return BadRequest("邮政编码填写错误。如果不知道，且通讯地址在信宜市请填写525300");
                }
                if (Obj.parent_phone_number.Length != 11)
                {
                    return BadRequest("家长手机号码填写错误，请重新填写");
                }
                try
                {
                    Convert.ToInt64(Obj.parent_phone_number);
                }
                catch
                {
                    return BadRequest("家长手机号码填写错误，请重新填写");
                }
                var birthday = Obj.id_number.Substring(6, 4) + "-" + Obj.id_number.Substring(10, 2) + "-" + Obj.id_number.Substring(12, 2);
                var sex = Obj.id_number.Substring(14, 3);
                Obj.YYYYMMDD = birthday;
                if (int.Parse(sex) % 2 == 0)//性别代码为偶数是女性奇数为男性
                {
                    Obj.GD = "女";
                }
                else
                {
                    Obj.GD = "男";
                }
                Program.Log(sex + " " + Obj.GD);
                string outputNames = "output\\" + Obj.id_number + ".docx";
                System.IO.File.Copy("template.docx", outputNames, true);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(outputNames, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    // 获取所有文本节点
                    var texts = body.Descendants<Text>();

                    // 替换单值字段
                    ReplaceText(texts, "college", Obj.college_name);
                    ReplaceText(texts, "departmentname", Obj.deparment_name);
                    ReplaceText(texts, "majorname", Obj.major_name);
                    ReplaceText(texts, "grade", Obj.grade);
                    ReplaceText(texts, "familyNums", Obj.family_num);
                    ReplaceText(texts, "class", Obj.@class);
                    ReplaceText(texts, "NAME", Obj.NAME);
                    ReplaceText(texts, "GD", Obj.GD);
                    ReplaceText(texts, "YYYY-MM-DD", Obj.YYYYMMDD);
                    ReplaceText(texts, "id_number", Obj.id_number);
                    ReplaceText(texts, "address", Obj.address);
                    ReplaceText(texts, "postcode", Obj.postcode);
                    ReplaceText(texts, "parent_phone_number", Obj.parent_phone_number);
                    ReplaceText(texts, "avg_ai", Obj.avg_ai);
                    ReplaceText(texts, "stu_phone_number", Obj.stu_phone_number);
                    if (Obj.s1.Replace(" ", "") == "") Obj.s1 = Program.s1;
                    if (Obj.s2.Replace(" ", "") == "") Obj.s2 = Program.s2;
                    if (Obj.s3.Replace(" ", "") == "") Obj.s3 = Program.s3;
                    if (Obj.s4.Replace(" ", "") == "") Obj.s4 = Program.s4;
                    if (Obj.s5.Replace(" ", "") == "") Obj.s5 = Program.s5;
                    if (Obj.s6.Replace(" ", "") == "") Obj.s6 = Program.s6;
                    ReplaceText(texts, "s1", Obj.s1);
                    ReplaceText(texts, "s2", Obj.s2);
                    ReplaceText(texts, "s3", Obj.s3);
                    ReplaceText(texts, "s4", Obj.s4);
                    ReplaceText(texts, "s5", Obj.s5);
                    ReplaceText(texts, "s6", Obj.s6);

                    // 家庭成员替换，最多6个
                    for (int i = 0; i < 6; i++)
                    {
                        var member = i < Obj.family_members.Count ? Obj.family_members[i] : new Family_membersItem();

                        ReplaceText(texts, $"pname", member.Name ?? "");
                        ReplaceText(texts, $"Age", member.Age ?? "");
                        ReplaceText(texts, $"Rs", member.Rs ?? "");
                        ReplaceText(texts, $"Workplace", member.Workplace ?? "");
                        ReplaceText(texts, $"Pro", member.Pro ?? "");
                        ReplaceText(texts, $"Ai", member.Ai ?? "");
                        ReplaceText(texts, $"Health", member.Health ?? "");
                    }

                    doc.MainDocumentPart.Document.Save();
                }

            }
            catch {
                return BadRequest("转换成文档失败。请检查数据是否完整");
            }
            return Ok(request);
        }

        private static void ReplaceText(IEnumerable<Text> texts, string placeholder, string value)
        {
            foreach (var t in texts)
            {
                if (t.Text.Contains(placeholder))
                {
                    t.Text = t.Text.Replace(placeholder, value ?? "");
                    return;
                }
            }
        }
    }

    public class Family_membersItem
    {

        public string Name { get; set; }

        public string Age { get; set; }

        public string Rs { get; set; }

        public string Workplace { get; set; }
 
        public string Pro { get; set; }
 
        public string Ai { get; set; }
 
        public string Health { get; set; }
    }

    public class Root
    {

        public string college_name { get; set; }

        public string deparment_name { get; set; }

        public string major_name { get; set; }

        public string grade { get; set; }

        public string @class { get; set; }

        public string NAME { get; set; }

        public string GD { get; set; }

        public string YYYYMMDD { get; set; }

        public string id_number { get; set; }

        public string family_num { get; set; }

        public string stu_phone_number { get; set; }

        public string address { get; set; }

        public string postcode { get; set; }

        public string parent_phone_number { get; set; }

        public List<Family_membersItem> family_members { get; set; }

        public string avg_ai { get; set; }

        public string s1 { get; set; }

        public string s2 { get; set; }

        public string s3 { get; set; }

        public string s4 { get; set; }

        public string s5 { get; set; }

        public string s6 { get; set; }
    }
}

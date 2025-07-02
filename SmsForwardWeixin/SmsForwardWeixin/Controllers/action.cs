using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using System.Xml.Linq;
using System.Xml;

namespace SmsForwardWeixin.Controllers
{
    public class action : Controller
    {
        [HttpPost("action")]
        public async Task<IActionResult> Index()
        {
            bool isVerify = false;
            using StreamReader reader = new StreamReader(Request.Body);
            string text = await reader.ReadToEndAsync();
            Console.WriteLine(text);
            XDocument doc = XDocument.Parse(text);
            string fromUsers = (string)doc.Root.Element("FromUserName");
            string contents = (string)doc.Root.Element("Content");
            string ctms = (string)doc.Root.Element("CreateTime");
            if (fromUsers == "o2-vT7ZaixzCHgZZ57lks_xgyqE4" || fromUsers == "o2-vT7elz_cP5daIFfwWx4UeMNpc")
            {
                isVerify = true;
            }
            else
            {
                return Ok("");
            }
            XDocument reply = new XDocument(
    new XElement("xml",
        new XElement("ToUserName", new XCData(fromUsers)),
        new XElement("FromUserName", new XCData("gh_29fa404ee99e")),
        new XElement("CreateTime", ctms),
        new XElement("MsgType", new XCData("text")),
        new XElement("Content", new XCData(Program.MessageData))

    )
);
            return Ok(reply.ToString());
        }
        [HttpGet("action")]
        public async Task<IActionResult> weixinVerify()
        {
            var returnData = Request.Query["echostr"].ToString();
            return Ok(returnData);
        }
        [HttpPost("saveMessage")]
        public async Task<IActionResult> saveMessage()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            string text = await reader.ReadToEndAsync();
            Program.MessageData = text;
            return Ok(text);
        }
    }
}

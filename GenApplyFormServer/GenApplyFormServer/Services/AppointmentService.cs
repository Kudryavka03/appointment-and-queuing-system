using GenApplyFormServer.Data;
using GenApplyFormServer.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GenApplyFormServer.Services
{
    public class AppointmentService
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AppointmentController : ControllerBase
        {
            private readonly AppDbContext _context;
            public AppointmentController(AppDbContext context)
            {
                _context = context;
            }

            // 添加预约时段
            [HttpPost("config")]
            public async Task<IActionResult> AddConfig([FromBody] Config config)
            {
                _context.Configs.Add(config);
                await _context.SaveChangesAsync();
                return Ok(config);
            }

            // 查询所有时段
            [HttpGet("config")]
            public async Task<IActionResult> GetConfigs()
            {
                var configs = await _context.Configs
                    .Include(c => c.Appointments)
                    .ToListAsync();
                return Ok(configs.Select(c => new
                {
                    c.Id,
                    c.Date,
                    c.StartTime,
                    c.EndTime,
                    c.MaxNum,
                    CurrentNum = c.Appointments.Count,
                    Remain = c.MaxNum - c.Appointments.Count
                }));
            }

            // 创建预约
            [HttpPost]
            public async Task<IActionResult> CreateAppointment([FromQuery] int configId)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                var config = await _context.Configs
                    .Include(c => c.Appointments)
                    .FirstOrDefaultAsync(c => c.Id == configId);

                if (config == null) return NotFound("配置不存在");

                if (config.Appointments.Count >= config.MaxNum)
                    return BadRequest("预约人数已满");

                var verifyCode = Guid.NewGuid().ToString("N")[..6].ToUpper();

                var appointment = new Appointment
                {
                    ConfigId = configId,
                    VerifyCode = verifyCode
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "预约成功", VerifyCode = verifyCode });
            }

            // 删除预约
            [HttpDelete("{verifyCode}")]
            public async Task<IActionResult> DeleteAppointment(string verifyCode)
            {
                var appt = await _context.Appointments.FirstOrDefaultAsync(a => a.VerifyCode == verifyCode);
                if (appt == null) return NotFound("预约不存在");

                _context.Appointments.Remove(appt);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "删除成功" });
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GenApplyFormServer.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string VerifyCode { get; set; } = string.Empty;

        public Config Config { get; set; } = null!;
    }
}

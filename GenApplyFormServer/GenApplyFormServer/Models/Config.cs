using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenApplyFormServer.Models
{
    public class Config
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int MaxNum { get; set; }

        // 导航属性
        public ICollection<Appointment> Appointments { get; set; }
    }

}

using HRSupport.Domain.Common;
using System;

namespace HRSupport.Domain.Entites
{
    public class Log : BaseEntity
    {
        public string ActionName { get; set; } // Örn: "CreateEmployeeCommand"
        public string UserId { get; set; }     // İşlemi yapan kullanıcı
        public string Details { get; set; }    // İşlem verileri (JSON formatında)
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string IpAddress { get; set; }  // İşlemin geldiği IP
    }
}
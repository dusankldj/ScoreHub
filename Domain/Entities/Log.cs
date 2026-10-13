using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Log : BaseEntity.BaseEntity
    {
        public string? Exception { get; set; }
        public string? InnerException { get; set; }
        public string? StackTrace { get; set; }
        public long? UserId { get; set; }
        public string? HttpMethod { get; set; }
        public string? HttpUrl { get; set; }
        public int? StatusCode { get; set; }
    }
}

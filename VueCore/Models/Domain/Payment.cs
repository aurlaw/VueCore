using System;

namespace VueCore.Models.Domain
{
    public class Payment
    {
        public string Id {get;set;} = default!;
        public string UserId {get;set;} = default!;
        public decimal Amount {get;set;}
        public string Status {get;set;} = default!;
        public DateTime ScheduledAt {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}

    }
}
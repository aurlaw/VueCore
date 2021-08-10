using System;
namespace VueCore.Models.Domain
{
    public class Document
    {
        public string Id {get;set;} = default!;
        public string Name {get;set;} = default!;
        public string Notes {get;set;} = default!;
        public string FileUrl {get;set;} = default!;
        public DateTime CreatedAt {get;set;}

    }
}
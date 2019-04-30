
using SQLite;
using System;

namespace CoreXF
{
    [Table(nameof(TableName))]
    public class HttpCacheModel 
    {
        public const string TableName = "httpcachemodel";

        [PrimaryKey]
        public string Key { get; set; }
    
        public string Content { get; set; }

        public DateTimeOffset Started { get; set; }
        //public DateTimeOffset Expired { get; set; }

    }

}

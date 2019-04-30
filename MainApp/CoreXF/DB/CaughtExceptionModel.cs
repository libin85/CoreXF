

using SQLite;
using System;

namespace CoreXF
{
    [Table(nameof(Table))]
    public class CaughtExceptionModel
    {
        public const string Table = "caughtexceptions";

        [PrimaryKey]
        public string Id { get; set; }

        public string Message { get; set; }

        public string Body { get; set; }

        public DateTimeOffset DateTime {get;set;}


    }
}

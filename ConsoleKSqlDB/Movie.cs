using ksqlDB.RestApi.Client.KSql.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleKSqlDB
{
    public class Movie : Record
    {
        public string Title { get; set; } = null!;
        public int Id { get; set; }
        public int Release_Year { get; set; }
    }
}

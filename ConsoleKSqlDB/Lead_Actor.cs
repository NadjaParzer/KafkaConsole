using ksqlDB.RestApi.Client.KSql.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleKSqlDB
{
    public class Lead_Actor : Record
    {
        public string Title { get; set; } = null!;
        public string Actor_Name { get; set; } = null!;
    }
}

using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamuraiApp.UI
{
    class SqlRawData
    {
        private readonly SamuraiContext _context;

        public SqlRawData(SamuraiContext context)
        {
            _context = context;
        }

        /// <summary>Queries using raw SQL strings.</summary>
        public void QueryUsingRawSql()
        {
            var samurais = _context.Samurais.FromSqlRaw("Select * from Samurais").ToList();
            samurais.ForEach(x => DisplaySamuraiName(x));
        }

        /// <summary>Queries using raw SQL with related data.</summary>
        public void QueryUsingRawSql_WithRelatedData()
        {
            var samurais = _context.Samurais.FromSqlRaw("Select * from Samurais").Include(s => s.Quotes).ToList();
            samurais.ForEach(x => DisplaySamuraiName(x, true));
        }

        /// <summary>Queries using raw SQL with interpolated strings</summary>
        public void QueryUsingRawSql_Interpolated()
        {
            var name = "Kikuchyo";
            var samurais = _context.Samurais.FromSqlInterpolated($"Select * from Samurais WHERE name = {name}").ToList();
            samurais.ForEach(x => DisplaySamuraiName(x));
        }

        public void QueryUsingFromRawSqlStoredProc()
        {
            var text = "Happy";
            var samurais = _context.Samurais.FromSqlRaw("EXEC dbo.SamuraisWhoSaidAWord {0}", text).ToList();
            samurais.ForEach(x => DisplaySamuraiName(x));
        }
        public void QueryUsingFromRawSqlStoredProc_Interpolated()
        {
            var text = "Happy";
            var samurais = _context.Samurais.FromSqlInterpolated($"EXEC dbo.SamuraisWhoSaidAWord {text}").ToList();
            samurais.ForEach(x => DisplaySamuraiName(x));
        }

        private static void DisplaySamuraiName(Samurai samurai, bool showQuotes = false)
        {
            Console.WriteLine(samurai.Name);
            if (showQuotes && samurai.Quotes.Any())
                samurai.Quotes.ForEach(x => Console.WriteLine(x.Text));
        }

    }
}

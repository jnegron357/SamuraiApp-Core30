using System;
using SamuraiApp.Domain;
using SamuraiApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SamurayApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        static void Main()
        {
            //InsertSamurai();
            //InsertMultipleSamrais();
            //InsertMultipleDifferentObjects();
            //SimpleSamuraiQuery();
            //MoreQuries();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabseOperations();
            //InsertBattle();
        }
        private static void QueryAndUpdateBattle_Disconnected()
        {
            //Modified from training course to show data of updated object for confirmation.
            var battle = _context.Battles.FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);
            using (var newContextInstance = new SamuraiContext())
            {
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
                Console.WriteLine("Battle ID:" + battle.Id);
                Console.WriteLine("New battle End Date: " + battle.EndDate.ToShortDateString());
            }
        }

        private static void InsertBattle()
        {
            //Modified from training course to show data of newly inserted object for confirmation.
            var battle = new Battle
            {
                Name = "Battle of Okehazama",
                StartDate = new DateTime(1560, 05, 01),
                EndDate = new DateTime(1560, 06, 15)
            };
            _context.Battles.Add(battle);
            _context.SaveChanges();
            Console.WriteLine("New battle ID:" + battle.Id);
            Console.WriteLine("New battle Name: " + battle.Name);
        }

        private static void MultipleDatabseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "Hiro";
            _context.Samurais.Add(new Samurai { Name = "Kikuchiyo" });
            _context.SaveChanges();
        }

        private static void MoreQuries()
        {
            var samurais = _context.Samurais.Where(x => x.Name == "Joan Smith").ToList();
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
            GetAllSamurai();
        }


        private static void GetAllSamurai()
        {
            using var context = new SamuraiContext();
            //var samurais = context.Samurais.ToList();
            var query = context.Samurais;
            foreach (var samurai in query)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        private static void InsertMultipleSamrais()
        {
            var samurai1 = new Samurai { Name = "Samurai1" };
            var samurai2 = new Samurai { Name = "Samurai2" };
            var samurai3 = new Samurai { Name = "Samurai3" };
            var samurai4 = new Samurai { Name = "Samurai4" };
            using var context = new SamuraiContext();
            context.Samurais.AddRange(samurai1, samurai2, samurai3, samurai4);
            context.SaveChanges();
        }

        private static void InsertMultipleDifferentObjects()
        {
            var samurai = new Samurai { Name = "Oda Nobunaga" };
            var battle = new Battle
            {
                Name = "Battle of Nagashino",
                StartDate = new DateTime(1575, 06, 16),
                EndDate = new DateTime(1575, 06, 28)
            };
            using var context = new SamuraiContext();
            context.AddRange(samurai, battle);
            context.SaveChanges();
        }

        private static void InsertSamurai()
        {
            using var context = new SamuraiContext();
            context.Samurais.Add(new Samurai { Name = "Bob Smith" });
            //This also works as EFCore will know to use the Samurai table given the Type that's been added.
            //context.Add(new Samurai { Name = "John Doe" });
            context.SaveChanges();
        }
    }
}

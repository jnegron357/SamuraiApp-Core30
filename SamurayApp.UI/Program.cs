using System;
using SamuraiApp.Domain;
using SamuraiApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace SamurayApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        static void Main()
        {
            #region module 4 calls
            //InsertSamurai();
            //InsertMultipleSamrais();
            //InsertMultipleDifferentObjects();
            //SimpleSamuraiQuery();
            //MoreQuries();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabseOperations();
            //InsertBattle();
            //QueryAndUpdateBattle_Disconnected();
            //AddSomeMoreSamurais();
            //DeleteWhileTracked();
            //DeleteWhileNotTracked();
            //DeleteMany();
            //DeleteUsingId(4); 
            #endregion

            //InsertNewPkFkGraph();
            //InsertNewPkFkGraphMultipleChildren();
            //AddChildToExistingObjectWhileTracked();
            //AddChildToExistingObjectWhileNotTracked(7);
            //EagerLoadSamuraiWithQuotes();
            //ProjectSomeProperties();
            //ProjectSamuraisWithQuotes();
            //ProjectSamuraisWithQuotesFiltered();
            //ProjectSomeProperties();
            //ChildFilteringWithRelatedData();
            //ModifyingRelatedDataWhenTracked();
            //InsertVariousTypes();
            //QueryAndUpdateBattle_Disconnected();
            QueryAndUpdateBattle_Disconnected_AsNoTracking();
        }

        private static void QueryFilters()
        {
            var name = "Sampson";
            //var samurais = _context.Samurais.Where(s => s.Name == "Sampson").ToList();

            //Use find for getting an entity by ID. Benefit is that EF will pull it from memory if it is bieng tracked already.
            var samurai = _context.Samurais.Find(2);
            Console.WriteLine(samurai.Name);

            //Use the new Like function:
            var filter = "J%";
            var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, filter)).ToList();
            samurais.ForEach(x => Console.WriteLine(x.Name));

            //Use FirstOrDefault with lambda
            var samurai2 = _context.Samurais.FirstOrDefault(x => x.Name == name);
            Console.WriteLine(samurai2.Name);

            //Use LastOrDefault with lambda, requires OrderBy to called first!
            var last = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);
            Console.WriteLine(last.Name);
            
            //the following will throw an exception:
            //var lastNoOrder= _context.Samurais.LastOrDefault(s => s.Name == name);

            samurais.ForEach(x => Console.WriteLine(x.Name));
        }

        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "KikuchioSan" };
            var clan = new Clan { ClanName = "Galactic Clan" };
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
            Console.WriteLine($"Samurai id: {samurai.Id}, Clan Id: {clan.Id}");
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault();
            samurai.Quotes[0].Text += " Did you hear that?";
            _context.SaveChanges();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault();
            var quote = samurai.Quotes[0];
            quote.Text += " Did you hear that?";
            using (var newContext = new SamuraiContext())
            {
                //newContext.Quotes.Update(quote);
                newContext.Entry(quote).State = EntityState.Modified;
                newContext.SaveChanges();
            }
        }

        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                .ToList();
            samurais.ForEach(x => DisplaySamuraiName(x));
        }

        private static void ChildFilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Select(x => new 
                    { 
                        Samurai = x, 
                        Quotes = x.Quotes.Where(q => q.Text.Contains("happy")).ToList() 
                    })
                .ToList();
            //show me the results
            foreach (var item in samurais)
            {
                var name = item.Samurai.Name;
                item.Quotes.ForEach(q => Console.WriteLine($"{name} says: {q.Text}"));
            }
        }

        private static void DisplaySamuraiName(Samurai samurai, bool showQuotes = false)
        {
            Console.WriteLine(samurai.Name);
            if(showQuotes && samurai.Quotes.Any()) 
                samurai.Quotes.ForEach(x => Console.WriteLine(x.Text));
        }

        private static void ProjectSamuraisWithQuotes()
        {
            //Project to anonumous type
            var somePropertiesWithQuotes = _context.Samurais
                .Select(s => new { s.Id, s.Name, s.Quotes.Count })
                .ToList();
            somePropertiesWithQuotes.ForEach(x => Console.WriteLine($"Id: {x.Id}, Name: {x.Name}, Quotes: {x.Count}"));
        }

        private static void ProjectSamuraisWithQuotesFiltered()
        {
            //Project to anonumous type and filter by Count
            var somePropertiesWithQuotes = _context.Samurais
                .Select(s => new { s.Id, s.Name, s.Quotes.Count })
                .Where(x => x.Count > 0)
                .ToList();
            somePropertiesWithQuotes.ForEach(x => Console.WriteLine($"Id: {x.Id}, Name: {x.Name}, Quotes: {x.Count}"));
        }

        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id;
            public string Name;
        }

        private static void ProjectSomeProperties()
        {
            //anonymous type
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            //specific type
            var idsAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
            //output
            someProperties.ForEach(x => Console.WriteLine($"Id: {x.Id}, Name: {x.Name}"));
            idsAndNames.ForEach(x => Console.WriteLine($"Id: {x.Id}, Name: {x.Name}"));
        }

        private static List<dynamic> ProjectDynamic()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            return someProperties.ToList<dynamic>();
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais
                .Where(s => s.Name.Contains("Kyūzō"))
                .Include(s => s.Quotes)
                .Include(s => s.SecretIdentity)
                .FirstOrDefault();
        }

        private static void AddChildToExistingObjectWhileNotTracked(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?",
                SamuraiId = samuraiId
            };
            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }

        private static void AddChildToExistingObjectWhileTracked()
        {
            var samurai = _context.Samurais.First();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }

        private static void InsertNewPkFkGraphMultipleChildren()
        {
            var samurai = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote> {
                  new Quote {Text = "Watch out for my sharp sword!"},
                  new Quote {Text="I told you to watch out for the sharp sword! Oh well!" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewPkFkGraph()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "I've come to save you"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        #region mdule 4 methods

        private static void DeleteUsingId(int samuraiId)
        {
            //This is stil a two-trip operation :(
            var samurai = _context.Samurais.Find(samuraiId);
            _context.Remove(samurai);
            _context.SaveChanges();
            //alternate: call a stored procedure for a one-trip delete operation!
            //_context.Database.ExecuteSqlCommand("exec [your_sproc_name] {0}", samuraiId);
        }

        private static void AddSomeMoreSamurais()
        {
            _context.AddRange(
               new Samurai { Name = "Kambei Shimada" },
               new Samurai { Name = "Shichirōji " },
               new Samurai { Name = "Katsushirō Okamoto" },
               new Samurai { Name = "Heihachi Hayashida" },
               new Samurai { Name = "Kyūzō" },
               new Samurai { Name = "Gorōbei Katayama" }
             );
            _context.SaveChanges();
        }

        private static void DeleteMany()
        {
            var samurais = _context.Samurais.Where(s => s.Name.Contains("ō"));
            _context.Samurais.RemoveRange(samurais);
            //alternate: _context.RemoveRange(samurais);

            //Modified from original to show rows affected
            var rowsAffected = _context.SaveChanges();
            Console.WriteLine($"Rows affected: {rowsAffected}");
        }

        private static void DeleteWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Shichirōji");
            _context.Samurais.Remove(samurai);
            //alternates:
            // _context.Remove(samurai);
            // _context.Entry(samurai).State=EntityState.Deleted;
            // _context.Samurais.Remove(_context.Samurais.Find(1));

            //Modified from original to show rows affected
            var rowsAffected = _context.SaveChanges();
            Console.WriteLine($"Rows affected: {rowsAffected}");
        }

        private static void DeleteWhileNotTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Heihachi Hayashida");
            using (var contextNewAppInstance = new SamuraiContext())
            {
                contextNewAppInstance.Samurais.Remove(samurai);
                //contextNewAppInstance.Entry(samurai).State=EntityState.Deleted;
                contextNewAppInstance.SaveChanges();
            }
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

        private static void QueryAndUpdateBattle_Disconnected_AsNoTracking()
        {
            //In desconnected scenarios, ensure that the DB context doesn't create entity entry objects to track the results of the query. 
            //This will improve performance due to the fact that EF Core is not using any tracking objects during it's operation.
            //NOTE: AsNoTracking() returns a query not a DbSet!
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
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

        private static void RetrieveAndUpdateMultipleSamurais_UsingSkipAndTake()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
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

        #endregion
    }
}

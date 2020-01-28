using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamuraiApp.UI
{
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

    public class RelatedData
    {
        private readonly SamuraiContext _context;

        public RelatedData(SamuraiContext context)
        {
            _context = context;
        }

        public void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                .ToList();
        }

        public void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                    {
                      new Quote { Text = "I've come to save you" }
                    }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
            Console.WriteLine($"Samurai {samurai.Name} added!");
        }

        public void InsertNewSamuraiWithManyQuotes()
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

        public void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }
        public void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });
            using var newContext = new SamuraiContext();
            newContext.Samurais.Attach(samurai);
            newContext.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiNotTracked_Easy(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinner again?",
                SamuraiId = samuraiId
            };
            using var newContext = new SamuraiContext();
            newContext.Quotes.Add(quote);
            newContext.SaveChanges();
        }

        #region Eager Loading

        public void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = 
                _context
                .Samurais
                .Where(s => s.Name.Contains("Julie"))
                .Include(s => s.Quotes)
                .Include(s => s.Clan)
                .FirstOrDefault();
        }

        public void GetSamuraiWithBattles()
        {
            var samuraiWithBattle = _context.Samurais
              .Include(s => s.SamuraiBattles)
              .ThenInclude(sb => sb.Battle)
              .FirstOrDefault(samurai => samurai.Id == 2);

            var samuraiWithBattlesCleaner = _context.Samurais.Where(s => s.Id == 2)
              .Select(s => new
              {
                  Samurai = s,
                  Battles = s.SamuraiBattles.Select(sb => sb.Battle)
              })
              .FirstOrDefault();
        }

        #endregion

        #region Explicit Loading
        
        public void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Julie"));
            //NOTE: You can only Load from a single object, not a collection.
            //Use the Collection method for public entities
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            //Use the Reference method for private entities
            _context.Entry(samurai).Reference(x => x.Hourse).Load();
        }

        #endregion

        #region Lazy Loading

        public void LazyLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Julie"));
            var quoteCount = samurai.Quotes.Count();
            Console.WriteLine($"Quote count: {quoteCount}");
        }

        #endregion

        public void ProjectSomeProperties()
        {
            //Project using anonymous type
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            //Project using defined type
            var idsAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
        }

        public void ProjectSamuraisWithQuotes()
        {
            //Project anonumous type with related data 
            var somePropertiesWithQuotes1 = _context.Samurais
               .Select(s => new { s.Id, s.Name, s.Quotes.Count })
               .ToList();
            somePropertiesWithQuotes1.ForEach(x => Console.WriteLine($"Id: {x.Id}, Name: {x.Name}, Quotes: {x.Count}"));            
        }

        public void ProjectSamuraisWithQuotes_filtered()
        {
            //Project anonymous type with filtered related data
            var somePropertiesWithQuotes2 = _context.Samurais
               .Select(s => new
               {
                   s.Id,
                   s.Name,
                   HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
               })
               .ToList();
            somePropertiesWithQuotes2.ForEach(x => Console.WriteLine($"Id: {x.Id}, Name: {x.Name}"));
        }

        public void ProjectSamuraisWithQuotes_filtered(bool showTracking)
        {
            //Project anonymous type with filtered related data and separate Samurai object
            var samuraisWithHappyQuotes = _context.Samurais
               .Select(s => new {
                   Samurai = s,
                   HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
               })
               .ToList();
            samuraisWithHappyQuotes.ForEach(x => Console.WriteLine(x.Samurai.Name));
            
            if (showTracking)
            {
                //Example of how entities are bieng tracked even when they're retured by the results of a projection query
                var firstSamurai = samuraisWithHappyQuotes[0].Samurai.Name += " The Happiest";
                _context.SaveChanges();
                Console.WriteLine($"New Samurai Name: {firstSamurai}");
            }
        }

        private static void DisplaySamuraiName(Samurai samurai, bool showQuotes = false)
        {
            Console.WriteLine(samurai.Name);
            if (showQuotes && samurai.Quotes.Any())
                samurai.Quotes.ForEach(x => Console.WriteLine(x.Text));
        }

    }
}

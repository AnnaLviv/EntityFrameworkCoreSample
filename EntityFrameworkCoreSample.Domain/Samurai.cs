using System.Collections.Generic;

namespace EntityFrameworkCoreSample.Domain
{
    public class Samurai
    {
        public Samurai()
        {
            Quotes = new List<Quote>();
            SamuraiBattles = new List<SamuraiBattle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Quote>   Quotes { get; set; }
        public Clan Clan { get; set; }
        public List<SamuraiBattle> SamuraiBattles { get; set; }

        /// <summary>
        /// Dependent one-to-one relation. Horse is optional.
        /// Inn case Horse is required - we have to handle this case in the business logic.
        /// </summary>
        public Horse Horse { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using travelapp.Model;

namespace travelapp.Persistance.MsSql
{
    public interface IDataProvider
    {
        public void SeedDatabase(string jsonPath);
        public trDbContext ctx { get; }
        public IEnumerable<Destination> GetAllDestinations();
        public IEnumerable<Airline> GetAllAirlines();
        public Airline GetAirlineByName(string airlineName);
        public Destination GetDestination(string airlineName, string cityName);
        public void AddAirline(Airline airline);
        public void RemoveAirline(Airline airline);
        public void AddDestination(Destination destination, string airlineName);
        public void RemoveDestination(Destination destination);
        public void SaveChangesDB();
    }
    public class DataProvider : IDataProvider
    {
        trDbContext ctx;

        public DataProvider(trDbContext ctx, string jsonPath)
        {
            this.ctx = ctx;
            SeedDatabase(jsonPath);
        }
        public void SeedDatabase(string jsonPath)
        {
            List<Airline> newAirlines = JsonConvert.DeserializeObject<List<Airline>>(File.ReadAllText(jsonPath));
            var existingAirlines = ctx.Airlines.Include(a => a.destinations).ToList();

            foreach (var newAirline in newAirlines)
            {
                var existingAirline = existingAirlines.FirstOrDefault(a => a.airlineName == newAirline.airlineName);

                if (existingAirline != null) //létezik-e már ez az airline
                {
                    foreach (var destination in newAirline.destinations) //ha igen akkor végigmegyünk a hozzáadandó(létező) airline destinationsein
                    {
                        if (!existingAirline.destinations.Any(d => d.city == destination.city || d.popularity == destination.popularity)) //ha nem egyezik meg a destination a létező airline egyik destinationjével sem, akkor hozzáadjuk
                        {
                            destination.airlineName = existingAirline.airlineName;
                            existingAirline.destinations.Add(destination);
                        }
                    }
                }
                else
                {
                    foreach (var destination in newAirline.destinations)
                    {
                        destination.airlineName = newAirline.airlineName;
                    }
                    ctx.Airlines.Add(newAirline);
                }
            }
            ctx.SaveChanges();
        }

        trDbContext IDataProvider.ctx { get { return ctx; } }

        public Airline GetAirlineByName(string airlineName)
        {
            return ctx.Airlines.FirstOrDefault(t => t.airlineName == airlineName);
        }

        public IEnumerable<Airline> GetAllAirlines()
        {
            return ctx.Airlines.ToList();
        }

        public IEnumerable<Destination> GetAllDestinations()
        {
            return ctx.Destinations.ToList();
        }

        public Destination GetDestination(string airlineName, string cityName)
        {
            return ctx.Destinations.FirstOrDefault(t => (t.airlineName == airlineName && t.city == cityName));
        }

        public void AddAirline(Airline airline)
        {
            ctx.Airlines.Add(airline);
            ctx.SaveChanges();
        }

        public void RemoveAirline(Airline airline)
        {
            ctx.Airlines.Remove(airline);
            ctx.SaveChanges();
        }

        public void AddDestination(Destination destination, string airlineName)
        {
            //var airline = GetAirlineByName(airlineName);
            //airline.destinations.Add(destination);
            ctx.Destinations.Add(destination);
            ctx.SaveChanges();
        }

        public void RemoveDestination(Destination destination)
        {
            ctx.Destinations.Remove(destination);
            ctx.SaveChanges();
        }

        public void SaveChangesDB()
        {
            ctx.SaveChanges();
        }
    }
}

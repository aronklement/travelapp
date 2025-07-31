using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static travelapp.FlightsService;
using travelapp.Model;
using travelapp.Persistance.MsSql;

namespace travelapp
{
    public interface IFlightsService
    {
        public string ToTable<T>(IEnumerable<T> items);
        public string ToTable2<T>(T item);
        public IDataProvider dtp { get; }
        public event EventHandler<NewCheapestEventArgs> NewCheapest;

        public IEnumerable<Destination> GetAllDestinations();
        public IEnumerable<Airline> GetAllAirlines();
        public Airline GetAirlineByName(string airlineName);
        public Destination GetDestination(string airlineName, string cityName);
        public void AddAirline(Airline airline);
        public void RemoveAirline(Airline airline);
        public void AddDestination(Destination destination, string airlineName);
        public void RemoveDestination(Destination destination);
        public void ModifyAirlineName(string airlineName, string newName);
        public IEnumerable<Destination> Search(string cityName, float maxPrice, int maxDistance);
        public IEnumerable<Destination> SearchBy2(float maxPrice, int maxDistance);
        public IEnumerable<Destination> SearchBy2(string cityName, float maxPrice);
        public IEnumerable<Destination> SearchByCityContains(string searchedString);
        public IEnumerable<Destination> SearchByCityEquals(string cityName);
        public IEnumerable<Destination> SearchByDistance(int maxDistance);
        public IEnumerable<Destination> SearchByPrice(float maxPrice);
        public void Report();
        public void Report(string path);

    }
    public class FlightsService : IFlightsService
    {
        public class NewCheapestEventArgs : EventArgs
        {
            public NewCheapestEventArgs(string city, float price, string airlineName)
            {
                City = city;
                Price = price;
                AirlineName = airlineName;
            }

            public string City { get; }
            public float Price { get; }
            public string AirlineName { get; }
        }
        public event EventHandler<NewCheapestEventArgs> NewCheapest;
        IDataProvider dtp;

        public FlightsService(IDataProvider dtp)
        {
            this.dtp = dtp;
        }

        IDataProvider IFlightsService.dtp { get { return dtp; } }

        public void AddAirline(Airline airline)
        {
            if (dtp.GetAirlineByName(airline.airlineName) == null)
            {
                dtp.AddAirline(airline);
            }
            else
            {
                throw new AirlineAlreadyExists(airline.airlineName);
            }
        }

        public void AddDestination(Destination destination, string airlineName)
        {
            destination.airlineName = airlineName;
            if (dtp.GetDestination(destination.airlineName, destination.city) == null)
            {
                var airline = GetAirlineByName(airlineName);
                airline.destinations.Add(destination);
                dtp.AddDestination(destination, airlineName);
                if (CheapestFlight(destination))
                {
                    NewCheapest?.Invoke(this, new NewCheapestEventArgs(destination.city, destination.price, destination.airlineName));
                }
            }
            else
            {
                throw new DestinationAlreadyExists(destination.airlineName, destination.city);
            }
        }
        public bool CheapestFlight(Destination destination)
        {
            bool cheapest = true;
            var flights = dtp.GetAllDestinations().Where(d => d.airlineName == destination.airlineName).ToList();
            foreach (var flight in flights)
            {
                if (flight.price <= destination.price)
                {
                    cheapest = false;
                }
            }
            return cheapest;
        }
        public Airline GetAirlineByName(string airlineName)
        {
            var q = dtp.GetAirlineByName(airlineName);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new AirlineNotFound(airlineName);
            }
        }

        public IEnumerable<Airline> GetAllAirlines()
        {
            var q = dtp.GetAllAirlines();
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoAirlines();
            }
        }

        public IEnumerable<Destination> GetAllDestinations()
        {
            var q = dtp.GetAllDestinations();
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }

        public Destination GetDestination(string airlineName, string city)
        {
            var q = dtp.GetDestination(airlineName, city);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new DestinationNotFound(airlineName, city);
            }
        }

        public void ModifyAirlineName(string airlineName, string newName)
        {
            var airline = dtp.GetAirlineByName(airlineName);
            if (airline != null)
            {
                var destinations = dtp.GetAllDestinations().Where(d => d.airlineName == airlineName);
                if (destinations != null && destinations.Any())
                {
                    foreach (var destination in destinations)
                    {
                        RemoveDestination(destination);
                    }
                    airline.airlineName = newName;
                    dtp.SaveChangesDB();
                    foreach (var destination in destinations)
                    {
                        AddDestination(destination, airline.airlineName);
                    }
                }
                dtp.SaveChangesDB();
            }
            else
            {
                throw new AirlineNotFound(airlineName);
            }
        }

        public void RemoveAirline(Airline airline)
        {
            if (dtp.GetAirlineByName(airline.airlineName) != null)
            {
                dtp.RemoveAirline(airline);
            }
            else
            {
                throw new AirlineNotFound(airline.airlineName);
            }
        }

        public void RemoveDestination(Destination destination)
        {
            if (dtp.GetDestination(destination.airlineName, destination.city) != null)
            {
                dtp.RemoveDestination(destination);
                var airline = GetAirlineByName(destination.airlineName);
                airline.destinations.Remove(destination);
                dtp.SaveChangesDB();
            }
            else
            {
                throw new DestinationNotFound(destination.airlineName, destination.city);
            }
        }
        public string ToTable2<T>(T item)
        {
            if (item != null)
            {
                string text = "";
                Type t = typeof(T);

                foreach (var prop in t.GetProperties().Where(p => !p.Name.Equals("ID") && !p.Name.Equals("airlineID")))
                {
                    text += prop.Name + "\t\t";
                }
                text += "\n";
                foreach (var prop in t.GetProperties().Where(p => !p.Name.Equals("ID") && !p.Name.Equals("airlineID")))
                {
                    text += prop.GetValue(item);
                    if (prop.Name == "distance")
                    {
                        text += "km";
                    }
                    else if (prop.Name == "price")
                    {
                        text += "EUR";
                    }

                    if (prop.GetValue(item).ToString().Length > 12)
                    {
                        text += "\t";
                    }
                    else
                    {
                        text += "\t\t";
                    }
                }
                text += "\n";
                return text;
            }
            else
            {
                return "No data available";
            }
        }
        public string ToTable<T>(IEnumerable<T> items)
        {
            if (items != null)
            {
                string text = "";
                Type t = typeof(T);

                foreach (var prop in t.GetProperties().Where(p => !p.Name.Equals("ID") && !p.Name.Equals("destinationID") && !p.Name.Equals("destinations")))
                {
                    text += prop.Name + "\t\t";
                }
                text += "\n";
                foreach (var item in items)
                {
                    foreach (var prop in t.GetProperties().Where(p => !p.Name.Equals("ID") && !p.Name.Equals("destinationID") && !p.Name.Equals("destinations")))
                    {
                        text += prop.GetValue(item);
                        if (prop.Name == "distance")
                        {
                            text += "km";
                        }
                        else if (prop.Name == "price")
                        {
                            text += "EUR";
                        }

                        if (prop.GetValue(item).ToString().Length > 12)
                        {
                            text += "\t";
                        }
                        else
                        {
                            text += "\t\t";
                        }
                    }
                    text += "\n";
                }
                return text;
            }
            else
            {
                return "No data available";
            }
        }
        public void Report()
        {
            var q1 = dtp.GetAllDestinations()
                        .Where(t => t.discounted == true)
                        .GroupBy(t => t.airlineName)
                        .Select(t => new
                        {
                            Key = t.Key,
                            DiscountedCount = t.Count()
                        });

            var q2 = dtp.GetAllDestinations().GroupBy(t => t.airlineName).Select(t => new
            {
                Key = t.Key,
                AveragePrice = t.Average(z => z.price),
                MinPrice = t.Min(z => z.price),
                MaxPrice = t.Max(z => z.price)
            });

            var q3 = dtp.GetAllDestinations().Where(t => t.popularity == 1).Select(t => new
            {
                AirlineName = t.airlineName,
                CityName = t.city
            });

            string report = (q1 != null ? ToTable(q1) : "No discounted flights") + "\n\n" +
                            (q2 != null ? ToTable(q2) : "No destinations") + "\n\n" +
                            (q3 != null ? ToTable(q3) : "No most popular destination");
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "report.txt"), report);
        }
        public void Report(string path)
        {
            var q1 = dtp.GetAllDestinations()
                        .Where(t => t.discounted == true)
                        .GroupBy(t => t.airlineName)
                        .Select(t => new
                        {
                            Key = t.Key,
                            DiscountedCount = t.Count()
                        });

            var q2 = dtp.GetAllDestinations().GroupBy(t => t.airlineName).Select(t => new
            {
                Key = t.Key,
                AveragePrice = t.Average(z => z.price),
                MinPrice = t.Min(z => z.price),
                MaxPrice = t.Max(z => z.price)
            });

            var q3 = dtp.GetAllDestinations().Where(t => t.popularity == 1).Select(t => new
            {
                AirlineName = t.airlineName,
                CityName = t.city
            });

            string report = (q1 != null && q1.Any() ? ToTable(q1) : "No discounted flights") + "\n\n" +
                            (q2 != null && q2.Any() ? ToTable(q2) : "No destinations") + "\n\n" +
                            (q3 != null && q3.Any() ? ToTable(q3) : "No most popular destination");
            File.WriteAllText(Path.Combine(path, "report.txt"), report);
        }
        public IEnumerable<Destination> SearchByPrice(float maxPrice)
        {
            var q = dtp.GetAllDestinations().Where(t => t.price <= maxPrice);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
        public IEnumerable<Destination> SearchByDistance(int maxDistance)
        {
            var q = dtp.GetAllDestinations().Where(t => t.distance <= maxDistance);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
        public IEnumerable<Destination> SearchByCityEquals(string cityName)
        {
            var q = dtp.GetAllDestinations().Where(t => t.city.Equals(cityName, StringComparison.OrdinalIgnoreCase));
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
        public IEnumerable<Destination> SearchByCityContains(string searchedString)
        {
            var q = dtp.GetAllDestinations().Where(t => t.city.Contains(searchedString, StringComparison.OrdinalIgnoreCase));
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
        public IEnumerable<Destination> SearchBy2(string cityName, float maxPrice) //Search by city and price
        {
            var q1 = SearchByCityContains(cityName);
            var q2 = SearchByPrice(maxPrice);
            var q = q1.Intersect(q2);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
        public IEnumerable<Destination> SearchBy2(float maxPrice, int maxDistance) //Search by price and distance
        {
            var q1 = SearchByDistance(maxDistance);
            var q2 = SearchByPrice(maxPrice);
            var q = q1.Intersect(q2);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
        public IEnumerable<Destination> Search(string cityName, float maxPrice, int maxDistance) //search city, price and distance
        {
            var q1 = SearchByCityContains(cityName);
            var q2 = SearchByPrice(maxPrice);
            var q3 = SearchByDistance(maxDistance);
            var q = q1.Intersect(q2).Intersect(q3);
            if (q != null)
            {
                return q;
            }
            else
            {
                throw new NoDestinations();
            }
        }
    }
}
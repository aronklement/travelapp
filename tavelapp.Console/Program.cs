using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using travelapp.Model;
using System;
using travelapp;
using travelapp.Persistance.MsSql;


namespace tavelappConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting application...");
            Console.WriteLine();

            string jsonPath = "data.json";

            var host = Host.CreateDefaultBuilder()
                            .ConfigureServices((hostContext, services) =>
                            {
                                services.AddScoped<trDbContext>();
                                services.AddSingleton<string>(jsonPath);
                                services.AddSingleton<IDataProvider, DataProvider>();
                                services.AddSingleton<IFlightsService, FlightsService>();
                            }).Build();

            host.Start();

            using IServiceScope serviceScope = host.Services.CreateScope();
            IFlightsService travelService = host.Services.GetRequiredService<IFlightsService>();

            travelService.NewCheapest += (sender, e) => Console.WriteLine($"New best deal to {e.City} just for {e.Price}EUR by {e.AirlineName}!"); //Subscribes method to NewCheapest event

            string[] commands = new string[] { "Search travels", "Make report", "Configure airlines", "Configure travels", "Exit" };
            bool end = false;
            while (!end)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Choose command by pressing a number!");
                Console.WriteLine();
                Console.WriteLine("------------------------------");
                for (int i = 0; i < commands.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {commands[i]}");
                }
                Console.WriteLine("------------------------------");
                Console.WriteLine();
                string[] a1 = { "1", "2", "3", "4", "5" };
                string a;
                do
                {
                    a = Console.ReadLine();
                    if (!a1.Contains(a))
                    {
                        Console.WriteLine("Command not found! Try again!");
                    }
                } while (!a1.Contains(a));

                switch (a)
                {
                    case "1":
                        string[] searches = {"Search by city name", "Search by price", "Search by distance",
                        "Search by price and distance", "Search by city name and price", "Search by city name, price and distance"};

                        Console.WriteLine();
                        Console.WriteLine("Select searching method: ");
                        Console.WriteLine();
                        Console.WriteLine("------------------------------");

                        for (int i = 0; i < searches.Length; i++)
                        {
                            Console.WriteLine($"{i + 1} - {searches[i]}");
                        }
                        Console.WriteLine("------------------------------");
                        Console.WriteLine();
                        string[] b1 = { "1", "2", "3", "4", "5", "6" };
                        string b;
                        do
                        {
                            b = Console.ReadLine();
                            if (!b1.Contains(b))
                            {
                                Console.WriteLine("Command not found! Try again!");
                            }
                        } while (!b1.Contains(b));
                        string cname;
                        string tmp;
                        float price;
                        int distance;
                        switch (b)
                        {
                            case "1":
                                Console.Write("City name: ");
                                cname = Console.ReadLine();
                                var flights = travelService.SearchByCityContains(cname);
                                Console.WriteLine(travelService.ToTable(flights));
                                break;
                            case "2":
                                Console.Write("Your budget(EUR): ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!float.TryParse(tmp, out price));
                                var flights2 = travelService.SearchByPrice(price);
                                Console.WriteLine(travelService.ToTable(flights2));
                                break;
                            case "3":
                                Console.Write("Biggest distance for your trip: ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!int.TryParse(tmp, out distance));
                                var flights3 = travelService.SearchByDistance(distance);
                                Console.WriteLine(travelService.ToTable(flights3));
                                break;
                            case "4":
                                Console.Write("Your budget(EUR): ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!float.TryParse(tmp, out price));
                                Console.Write("Biggest distance for your trip: ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!int.TryParse(tmp, out distance));
                                var flights4 = travelService.SearchBy2(price, distance);
                                Console.WriteLine(travelService.ToTable(flights4));
                                break;
                            case "5":
                                Console.Write("City name: ");
                                cname = Console.ReadLine();
                                Console.Write("Your budget(EUR): ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!float.TryParse(tmp, out price));
                                var flights5 = travelService.SearchBy2(cname, price);
                                Console.WriteLine(travelService.ToTable(flights5));
                                break;
                            case "6":
                                Console.Write("City name: ");
                                cname = Console.ReadLine();
                                Console.Write("Your budget(EUR): ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!float.TryParse(tmp, out price));
                                Console.Write("Biggest distance for your trip: ");
                                do
                                {
                                    tmp = Console.ReadLine();
                                } while (!int.TryParse(tmp, out distance));
                                var flights6 = travelService.Search(cname, price, distance);
                                Console.WriteLine(travelService.ToTable(flights6));
                                break;
                        }
                        Console.WriteLine();
                        Console.WriteLine();
                        break;
                    case "2":
                        Console.WriteLine();
                        Console.WriteLine("Select report method: ");
                        Console.WriteLine();
                        Console.WriteLine("------------------------------");
                        Console.WriteLine("1 - Make report to current directory");
                        Console.WriteLine("2 - Make report to path");
                        Console.WriteLine("------------------------------");
                        Console.WriteLine();
                        string[] c1 = { "1", "2" };
                        string c;
                        do
                        {
                            c = Console.ReadLine();
                            if (!c1.Contains(c))
                            {
                                Console.WriteLine("Command not found! Try again!");
                            }
                        } while (!c1.Contains(c));
                        if (c == "1")
                        {
                            travelService.Report();
                            Console.WriteLine("Report created in current directory!");
                        }
                        else
                        {
                            Console.Write("Add path for report: ");
                            string path = Console.ReadLine();
                            try
                            {
                                travelService.Report(path);
                            }
                            catch (System.IO.DirectoryNotFoundException e)
                            {
                                Console.WriteLine("Path not found!");
                            }

                        }
                        Console.WriteLine();
                        Console.WriteLine();
                        break;
                    case "3":
                        string[] airline_configs = { "Add airline", "Modify airline name", "Remove airline", "Get all airline names" };

                        Console.WriteLine();
                        Console.WriteLine("Select method: ");
                        Console.WriteLine();
                        Console.WriteLine("------------------------------");

                        for (int i = 0; i < airline_configs.Length; i++)
                        {
                            Console.WriteLine($"{i + 1} - {airline_configs[i]}");
                        }
                        Console.WriteLine("------------------------------");
                        Console.WriteLine();
                        string[] d1 = { "1", "2", "3", "4" };
                        string d;
                        do
                        {
                            d = Console.ReadLine();
                            if (!d1.Contains(d))
                            {
                                Console.WriteLine("Command not found! Try again!");
                            }
                        } while (!d1.Contains(d));
                        string airlineName;
                        switch (d)
                        {
                            case "1":
                                Console.Write("New airline's name: ");
                                airlineName = Console.ReadLine();
                                try
                                {
                                    travelService.AddAirline(new Airline(airlineName));
                                }
                                catch (AirlineAlreadyExists e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case "2":
                                Console.Write("Airline's name you wish to modify: ");
                                airlineName = Console.ReadLine();
                                Console.Write("New name: ");
                                string newName = Console.ReadLine();
                                try
                                {
                                    travelService.ModifyAirlineName(airlineName, newName);
                                }
                                catch (AirlineNotFound e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case "3":
                                Console.Write("Airline's name you wish to remove: ");
                                airlineName = Console.ReadLine();
                                try
                                {
                                    travelService.RemoveAirline(travelService.GetAirlineByName(airlineName));
                                }
                                catch (AirlineNotFound e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case "4":
                                try
                                {
                                    Console.WriteLine(travelService.ToTable(travelService.GetAllAirlines()));
                                }
                                catch (NoAirlines e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                                break;
                        }
                        break;
                    case "4":
                        string[] dest_configs = { "Add destination", "Remove destination", "Show all destinations" };

                        Console.WriteLine();
                        Console.WriteLine("Select method: ");
                        Console.WriteLine();
                        Console.WriteLine("------------------------------");

                        for (int i = 0; i < dest_configs.Length; i++)
                        {
                            Console.WriteLine($"{i + 1} - {dest_configs[i]}");
                        }
                        Console.WriteLine("------------------------------");
                        Console.WriteLine();
                        string[] f1 = { "1", "2", "3" };
                        string f;
                        do
                        {
                            f = Console.ReadLine();
                            if (!f1.Contains(f))
                            {
                                Console.WriteLine("Command not found! Try again!");
                            }
                        } while (!f1.Contains(f));

                        switch (f)
                        {
                            case "1":
                                string tmp2;
                                Console.WriteLine("Creating new destination..");
                                Console.Write("City name: ");
                                string cityName;
                                cityName = Console.ReadLine();
                                Console.Write("Country name: ");
                                string country;
                                country = Console.ReadLine();
                                int dist;
                                Console.Write("Distance from BUD: ");
                                do
                                {
                                    tmp2 = Console.ReadLine();
                                } while (!int.TryParse(tmp2, out dist));
                                float pric;
                                Console.Write("Price(EUR): ");
                                do
                                {
                                    tmp2 = Console.ReadLine();
                                } while (!float.TryParse(tmp2, out pric));
                                bool discounted;
                                Console.Write("Discounted (Y/N): ");
                                do
                                {
                                    tmp2 = Console.ReadLine();
                                } while (tmp2 != "Y" && tmp2 != "N");
                                if (tmp2 == "Y")
                                {
                                    discounted = true;
                                }
                                else
                                {
                                    discounted = false;
                                }
                                int popularity;
                                Console.Write("Popularity(has to be unique within the airline's destinations): ");
                                do
                                {
                                    tmp2 = Console.ReadLine();
                                } while (!int.TryParse(tmp2, out popularity));
                                string airlinename;
                                Console.Write("Airline name: ");
                                airlinename = Console.ReadLine();

                                try
                                {
                                    try
                                    {
                                        travelService.AddDestination(new Destination(cityName, country, dist, pric, discounted, popularity), airlinename);
                                    }
                                    catch (AirlineNotFound e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }

                                }
                                catch (DestinationAlreadyExists e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case "2":
                                Console.Write("City name: ");
                                string cityname = Console.ReadLine();
                                Console.Write("Airline name: ");
                                string AirlineName = Console.ReadLine();
                                try
                                {
                                    try
                                    {
                                        travelService.RemoveDestination(travelService.GetDestination(AirlineName, cityname));
                                    }
                                    catch (AirlineNotFound e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                                catch (DestinationNotFound e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case "3":
                                try
                                {
                                    Console.WriteLine(travelService.ToTable(travelService.GetAllDestinations()));
                                }
                                catch (NoDestinations e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                                break;
                        }
                        break;
                    case "5":
                        Console.WriteLine("Shutting down..");
                        end = true;
                        break;
                }
            }
        }
    }
}

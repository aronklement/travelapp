namespace travelapp
{
    public class DestinationNotFound : Exception
    {
        public DestinationNotFound(string airlineName, string city) : base($"Flight to {city} by {airlineName} does not exist!")
        {
        }
    }
    public class AirlineNotFound : Exception
    {
        public AirlineNotFound(string airlineName) : base($"Airline with the name '{airlineName}' does not exist!")
        {
        }
    }
    public class NoDestinations : Exception
    {
        public NoDestinations() : base("No destinations available.")
        {
        }
    }
    public class NoAirlines : Exception
    {
        public NoAirlines() : base("No airlines available.")
        {
        }
    }
    public class DestinationAlreadyExists : Exception
    {
        public DestinationAlreadyExists(string airlineName, string city) : base($"Flight to {city} by {airlineName} already exists!")
        {

        }
    }
    public class AirlineAlreadyExists : Exception
    {
        public AirlineAlreadyExists(string airlineName) : base($"Airline with the name {airlineName} already exists!")
        {

        }
    }
}

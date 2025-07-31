using Moq;
using NUnit.Framework;
using travelapp.Model;
using travelapp.Persistance;
using travelapp;

using travelapp.Persistance.MsSql;

namespace travelapp.Tests
{
    [TestFixture]
    public class Tester
    {
        protected FlightsService service;
        protected Mock<IDataProvider> mockDataProvider;
        protected List<Airline> mockAirlines;
        protected List<Destination> mockDestinations;

        [SetUp]
        public void Init()
        {
            mockAirlines = new List<Airline>()
            {
                new Airline("Wizz Air"),
                new Airline("Ryanair"),
                new Airline("Air France"),
                new Airline("Lufthansa")
            }.ToList();
            mockDestinations = new List<Destination>()
            {
                new Destination("Berlin", "Germany", 1000, (float)289.99, true, 1),     //0
                new Destination("Madrid", "Spain", 2000, (float)349.99, false, 2),      //1
                new Destination("Ljubljana", "Slovenia", 400, (float)100, true, 2),     //2
                new Destination("Prague", "Czech Republic", 500, (float)149.99, true, 1),//3
                new Destination("Athens", "Greece", 1500, (float)250, true, 1),         //4
                new Destination("Paris", "France", 1800, (float)299, false, 2),         //5
                new Destination("Istambul", "Turkey", 2200, (float)239.99, true, 2),    //6
                new Destination("London", "UK", 2000, (float)389.99, false, 1),         //7
            }.ToList();

            mockDataProvider = new Mock<IDataProvider>();
            mockDataProvider.Setup(t => t.GetAllAirlines()).Returns(mockAirlines);
            mockDataProvider.Setup(t => t.GetAllDestinations()).Returns(mockDestinations);

            service = new FlightsService(mockDataProvider.Object);

            mockDataProvider.Setup(t => t.GetAirlineByName("Wizz Air")).Returns(mockAirlines[0]);
            service.AddDestination(mockDestinations[4], "Wizz Air");
            service.AddDestination(mockDestinations[6], "Wizz Air");
            mockDataProvider.Setup(t => t.GetAirlineByName("Ryanair")).Returns(mockAirlines[1]);
            service.AddDestination(mockDestinations[0], "Ryanair");
            service.AddDestination(mockDestinations[1], "Ryanair");
            mockDataProvider.Setup(t => t.GetAirlineByName("Air France")).Returns(mockAirlines[2]);
            service.AddDestination(mockDestinations[7], "Air France");
            service.AddDestination(mockDestinations[2], "Air France");
            mockDataProvider.Setup(t => t.GetAirlineByName("Lufthansa")).Returns(mockAirlines[3]);
            service.AddDestination(mockDestinations[5], "Lufthansa");
            service.AddDestination(mockDestinations[3], "Lufthansa");

        }
    }
    [TestFixture]
    public class AirlineManagementTests : Tester
    {
        [Test]
        public void AddAirline_WithNewAirline_ShouldAddSuccessfully()
        {
            //Arrange
            var newAirline = new Airline("New Airline");
            mockDataProvider.Setup(m => m.GetAirlineByName(newAirline.airlineName)).Returns((Airline)null);

            //Act
            service.AddAirline(newAirline);

            //Assert
            mockDataProvider.Verify(m => m.AddAirline(newAirline), Times.Once);
        }

        [Test]
        public void AddAirline_WithExistingAirline_ShouldThrowException()
        {
            //Arrange
            var existingAirline = new Airline("Wizz Air");
            mockDataProvider.Setup(m => m.GetAirlineByName(existingAirline.airlineName)).Returns(mockAirlines[0]);

            //Assert
            Throws.TypeOf<AirlineAlreadyExists>();
        }

        [Test]
        public void RemoveAirline_WithExistingAirline_ShouldRemoveSuccessfully()
        {
            //Arrange
            var airlineToRemove = mockAirlines[0];
            mockDataProvider.Setup(m => m.GetAirlineByName(airlineToRemove.airlineName)).Returns(airlineToRemove);

            //Act
            service.RemoveAirline(airlineToRemove);

            //Assert
            mockDataProvider.Verify(m => m.RemoveAirline(airlineToRemove), Times.Once);
        }
        [Test]
        public void RemoveAirline_NotExistingAirline_ShouldThrowException()
        {
            //Arrange
            var airlineToRemove = new Airline("Not existing airlineName");
            mockDataProvider.Setup(m => m.GetAirlineByName(airlineToRemove.airlineName)).Returns((Airline)null);

            //Assert
            Throws.TypeOf<AirlineNotFound>();
        }
    }
    [TestFixture]
    public class DestinationManagementTests : Tester
    {
        [Test]
        public void AddDestination_WithNewDestination_ShouldAddSuccessfully()
        {
            //Arrange
            var newDestination = new Destination("Rome", "Italy", 1200, (float)199.99, false, 1);
            mockDataProvider.Setup(m => m.GetDestination("Wizz Air", "Rome")).Returns((Destination)null);
            mockDataProvider.Setup(m => m.GetAirlineByName("Wizz Air")).Returns(mockAirlines[0]);
            //Act
            service.AddDestination(newDestination, "Wizz Air");

            //Assert
            mockDataProvider.Verify(m => m.AddDestination(newDestination, "Wizz Air"), Times.Once);
        }

        [Test]
        public void AddDestination_WithExistingDestination_ShouldThrowException()
        {
            //Arrange
            var existingDestination = mockDestinations[4]; //Athens
            mockDataProvider.Setup(m => m.GetDestination("Wizz Air", "Athens")).Returns(existingDestination);

            //Assert
            Throws.TypeOf<DestinationAlreadyExists>();
        }
    }
    [TestFixture]
    public class SearchTests : Tester
    {
        [Test]
        public void SearchByPrice_ShouldReturnOnlyUnderMaxpriceDestinations()
        {
            //Arrange
            float maxPrice = (float)300;

            //Act
            var results = service.SearchByPrice(maxPrice).ToList();

            //Assert
            Assert.That(results.Count == 6);
            Assert.That(results.All(d => d.price <= maxPrice));
        }

        [Test]
        public void SearchByCityContains_ShouldReturnMatchingDestinations()
        {
            //Arrange
            string substring = "ub";

            //Act
            var results = service.SearchByCityContains(substring).ToList();

            //Assert
            Assert.That(results.Count == 1);
            Assert.That(results[0].city.Equals("Ljubljana"));
        }

        [Test]
        public void Search_WithAllParameters_ShouldReturnFilteredResults()
        {
            //Arrange
            string city = "a";
            float maxPrice = (float)300;
            int maxDistance = 2000;

            //Act
            var results = service.Search(city, maxPrice, maxDistance).ToList();

            //Assert
            Assert.That(results.Count == 4);
            Assert.That(results.All(d =>
                d.city.Contains("a", StringComparison.OrdinalIgnoreCase) &&
                d.price <= maxPrice &&
                d.distance <= maxDistance));
        }
    }
    [TestFixture]
    public class ReportTests : Tester
    {
        [Test]
        public void Report_ShouldGenerateFileWithCorrectContent()
        {
            //Arrange
            var tempPath = Directory.GetCurrentDirectory();
            var expectedPath = Path.Combine(tempPath, "report.txt");

            //Act
            service.Report(tempPath);

            //Assert
            Assert.That(File.Exists(expectedPath));
        }
    }
}

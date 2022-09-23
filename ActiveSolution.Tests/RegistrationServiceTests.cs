using ActiveSolution.Entities;
using Xunit;

namespace ActiveSolution.Tests
{
    public class RegistrationServiceTests
    {
        /*
            Note that the general formula for the price is 
            (costPerDay * days * firstFactor) + (secondFactor * costPerKm * kms)
            and where eg the firstFactor = 1 and seconFactor = 0 in the case of
            a Combi.
         */
        [Theory]
        [InlineData(VehicleCategory.SmallCar, 1200, 3, 1, 0, 100, 150)]
        [InlineData(VehicleCategory.Combi, 1200, 3, 1.3, 1, 100, 150)]
        [InlineData(VehicleCategory.Truck, 1200, 3, 1.5, 1.5, 100, 150)]
        public async Task CalculateCost(VehicleCategory category, int costPerDay, int days, 
                        double firstFactor, double secondFactor, int costPerKm, int kms)
        {
            var startDate = new DateTime(2022, 9, 1);
            var endDate = startDate.AddDays(days).AddSeconds(-1); // subtract a second as each initialezed 24 h period counts as a day.
            var bookingNumber = Guid.NewGuid().ToString();
            var registration = ConstructTestRentalRegistration(category, startDate, bookingNumber);
            var db = new MockRepository();

            var sut = new RegistrationService(db, costPerDay, costPerKm);
            await db.Create(registration);
            var actual = await sut.RegisterVehicleReturn(bookingNumber, endDate, kms);

            var expected = (costPerDay * days * firstFactor) + (secondFactor * costPerKm * kms);
            Assert.Equal(expected, actual, 15);
        }


        [Theory]
        [InlineData(VehicleCategory.SmallCar, "2022-09-1T09:00:00", "2022-09-2T08:59:00", 1*1)] // third inline value is expecte price per formula
        [InlineData(VehicleCategory.Combi, "2022-09-1T09:00:00", "2022-09-2T08:59:00", 1*1*1.3 + 1*1)]
        [InlineData(VehicleCategory.Truck, "2022-09-1T09:00:00", "2022-09-2T08:59:00", 1 * 1 * 1.5 + 1 * 1*1.5)]
        public async Task CalculateCostSecondVersion(VehicleCategory category, string start, string end, double expected)
        {
            var bookingNumber = Guid.NewGuid().ToString();
            var startDate = DateTime.Parse(start);
            var registration = ConstructTestRentalRegistration(category, startDate, bookingNumber);
            var endDate = DateTime.Parse(end);
            var db = new MockRepository();

            var sut = new RegistrationService(db, 1, 1); // cost perDay and cost per km is set to 1
            await db.Create(registration);
            var calculatedCost = await sut.RegisterVehicleReturn(bookingNumber, endDate, 1); // 1 km driven during rental

            Assert.Equal(expected, calculatedCost, 15);
        }

        [Fact]
        public async Task CostOfSmallCarDoesNotDependOnDistanceTravelled() // Test case could of course be covered by adding another line of data to the first test.
        {
            var startDate = new DateTime(2022, 9, 1);
            var endDate = startDate.AddDays(3).AddSeconds(-1); // subtract a second as each initialezed 24 h period counts as a day.
            var firstBookingNumber = Guid.NewGuid().ToString();
            var secondBookingNumber = Guid.NewGuid().ToString();
            var firstRegistration = ConstructTestRentalRegistration(VehicleCategory.SmallCar, startDate, firstBookingNumber);
            var secondRegistration = ConstructTestRentalRegistration(VehicleCategory.SmallCar, startDate, secondBookingNumber);
            var db = new MockRepository();

            var sut = new RegistrationService(db, 1000, 200);
            await db.Create(firstRegistration);
            await db.Create(secondRegistration);
            var first = await sut.RegisterVehicleReturn(firstBookingNumber, endDate, 100);
            var second = await sut.RegisterVehicleReturn(secondBookingNumber, endDate, 300);

            Assert.Equal(first, second, 15);
        }

        [Fact]
        public void RentalMustBeRegisteredBeforeReturn()
        {
            var db = new MockRepository();
            var sut = new RegistrationService(db, 100, 100);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sut.RegisterVehicleReturn("12345", DateTime.Now, 100)
                ) ;
        }    

        private static RentalRegistration ConstructTestRentalRegistration(VehicleCategory category, DateTime startDate, string bookingNumber)
        {
            var registration = new RentalRegistration(
                bookingNumber: bookingNumber,
                identificationNumber: "ABC-123",
                socialSecurityNumber: 980922,
                category: category,
                registrationDate: startDate,
                initialMeterSetting: 0
            );
            return registration;
        }
    }
}
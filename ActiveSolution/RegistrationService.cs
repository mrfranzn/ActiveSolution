using ActiveSolution.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSolution
{
    public class RegistrationService
    {
        public IRegistrationRepository Repository { get; }
        public int CostPerDay { get; }
        public int CostPerKm { get; }

        public RegistrationService(IRegistrationRepository repository, int costPerDay, int costPerKm)
        {
            Repository = repository;
            CostPerDay = costPerDay;
            CostPerKm = costPerKm;
        }

        public async Task<double> RegisterVehicleReturn(string bookingnumber, DateTime returnDate, int meterSetting)
        {
            var registration = await Repository.RegisterReturn(bookingnumber, returnDate, meterSetting);
            if (registration is null)
            {
                throw new KeyNotFoundException($"No Registration with booking number {bookingnumber} was found.");
            }
            // TODO: Handle that it should not be possible to return the same registration twice?

            var days = CalculateNumberOfDays(registration.RegistrationDate, returnDate);
            var kms = meterSetting - registration.InitialMeterSetting;

            var result = CalculateCost(registration, days, kms);

            return result;
        }

        private double CalculateCost(RentalRegistration registration, int numberOfDays, int distance)
        {
            var result = registration.Category switch
            {
                VehicleCategory.SmallCar => CostPerDay * numberOfDays,
                VehicleCategory.Combi => CostPerDay * numberOfDays * 1.3 + CostPerKm * distance,
                VehicleCategory.Truck => CostPerDay * numberOfDays * 1.5 + CostPerKm * distance * 1.5,
                _ => throw new ArgumentOutOfRangeException($"Not able to calculate price for {registration.Category}"),
            };
            return result;
        }

        private int CalculateNumberOfDays(DateTime start, DateTime end)
        {
            var span = end.Subtract(start).Days;
            var result = span + 1; // calculation starts from day one
            return result;
        }
    }
}

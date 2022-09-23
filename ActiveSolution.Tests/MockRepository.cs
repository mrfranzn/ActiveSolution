using ActiveSolution.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSolution.Tests
{
    public class MockRepository: Collection<RentalRegistration>, IRegistrationRepository
    {
        public Task Create(RentalRegistration registration)
        {
            Add(registration);
            return Task.CompletedTask;
        }

        public Task<RentalRegistration?> RegisterReturn(string bookingNumber, DateTime returnDate, int meterSetting)
        {
            var registration = this.FirstOrDefault(r => r.BookingNumber.Equals(bookingNumber));
            if (registration == null) return Task.FromResult<RentalRegistration?>(null);

            // Update registration in Collection
            registration.ReturnDate = returnDate;
            registration.FinalMeterSetting = meterSetting;

            return Task.FromResult<RentalRegistration?>(registration);
        }
    }
}

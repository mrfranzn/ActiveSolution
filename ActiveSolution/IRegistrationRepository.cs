using ActiveSolution.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSolution
{
    public interface IRegistrationRepository
    {
        Task Create(RentalRegistration registration);
        Task<RentalRegistration?> RegisterReturn(string bookingNumber, DateTime returnDate, int meterSetting);
    }
}

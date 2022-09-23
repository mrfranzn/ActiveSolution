using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSolution.Entities
{
    public class RentalRegistration
    {
        public RentalRegistration(
            string bookingNumber,
            string identificationNumber,
            int socialSecurityNumber,
            VehicleCategory category,
            DateTime registrationDate,
            int initialMeterSetting)
        {
            // TODO: set guards against initializing to invalid state (e.g. initialMeterSetting cannot be negative)
            BookingNumber = bookingNumber;
            IdentificationNumber = identificationNumber;
            SocialSecurityNumber = socialSecurityNumber;
            Category = category;
            RegistrationDate = registrationDate;
            InitialMeterSetting = initialMeterSetting;
        }
        public string BookingNumber { get; }
        public string IdentificationNumber { get; }
        public int SocialSecurityNumber { get; }
        public VehicleCategory Category { get; }
        public DateTime RegistrationDate { get; }
        public int InitialMeterSetting { get; }
        public DateTime? ReturnDate { get; set; } = null;
        public int? FinalMeterSetting { get; set; } = null;

        public override bool Equals(object? obj) // TODO: do we really need to compare RentalRegistration objects?
        {
            return obj is RentalRegistration registration &&
                BookingNumber == registration.BookingNumber &&
                IdentificationNumber == registration.IdentificationNumber &&
                SocialSecurityNumber == registration.SocialSecurityNumber &&
                Category == registration.Category &&
                RegistrationDate == registration.RegistrationDate &&
                InitialMeterSetting == registration.InitialMeterSetting &&
                ReturnDate == registration.ReturnDate &&
                FinalMeterSetting == registration.FinalMeterSetting;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                BookingNumber,
                IdentificationNumber,
                SocialSecurityNumber,
                Category,
                RegistrationDate,
                InitialMeterSetting,
                ReturnDate,
                FinalMeterSetting
                );
        }
    }
}

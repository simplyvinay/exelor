using System;
using System.Collections.Generic;
using System.Dynamic;
using Domain.Common;

namespace Application.Features.Users
{
    public class UserDetailsDto
    {
        protected UserDetailsDto()
        {
        }

        public UserDetailsDto(
            int id,
            string firstName,
            string lastName,
            string fullName,
            string email,
            string roles,
            CustomField[] customFields,
            AddressDetailDto address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            FullName = fullName;
            Email = email;
            Roles = roles;
            CustomFields = customFields.ToDto();
            Address = address;
        }

        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }
        public string Email { get; }
        public string Roles { get; }
        public AddressDetailDto Address { get; }
        public object CustomFields { get; set; }
    }

    public static class Helper
    {
        public static ExpandoObject ToDto(
            this CustomField[] customFields)
        {
            var x = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var customField in customFields)
            {
                x.Add(customField.Name, customField.Value);
            }
            return (ExpandoObject) x;
        }
    }

    public class AddressDetailDto
    {
        public AddressDetailDto(
            string address1,
            string address2,
            string street,
            string city,
            string state,
            string country,
            string zipCode)
        {
            Address1 = address1;
            Address2 = address2;
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }

        public string Address1 { get; }
        public string Address2 { get; }
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
    }
}
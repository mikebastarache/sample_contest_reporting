using System;
using System.ComponentModel.DataAnnotations;

namespace www.Models.ViewModels
{
    public class CrmUsersByProvinceView
    {
        [Key]
        public string Province { get; set; }
        public string ProvinceName { get; set; }
        public int TotalUsers { get; set; }
    }


    public class CrmUsersByGenderView
    {
        [Key]
        public int? Salutation { get; set; }
        public string SalutationEn { get; set; }
        public int TotalUsers { get; set; }
    }


    public class CrmUsersByYearOfBirthView
    {
        [Key]
        public string YearOfBirth { get; set; }
        public int TotalUsers { get; set; }
    }


    public class CrmUserView
    {
        [Key]
        public int UserId { get; set; }
        //public int? Salutation { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Telephone { get; set; }
        public string Language { get; set; }
        public string YearOfBirth { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
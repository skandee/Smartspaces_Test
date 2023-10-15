using App.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Dto
{
    public class CustomerDto
    {
        public string Firstname { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string EmailAddress { get; set; }

        public int CompanyId { get; set; }
    }
}

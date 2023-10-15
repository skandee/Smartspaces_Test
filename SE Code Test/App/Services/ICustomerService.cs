using App.Dto;
using System;

namespace App
{
    public interface ICustomerService
    {
        bool AddCustomer(CustomerDto customerDto);
        [Obsolete("Please use App.Services.CustomerService.AddCustomer(CustomerDto customerDto) instead")]
        bool AddCustomer(string firname, string surname, string email, DateTime dateOfBirth, int companyId);
    }
}

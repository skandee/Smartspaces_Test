using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using App.Entities;
using App.Enum;

//move this class into repository project
namespace App.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {   
        public void AddCustomer(Customer customer)
        {
            CustomerDataAccess.AddCustomer(customer);
        }
    }
}

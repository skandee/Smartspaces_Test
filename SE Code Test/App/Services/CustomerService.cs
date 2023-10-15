using System;
using App.Constants;
using App.Dto;
using App.Entities;
using App.Repositories;

namespace App
{
    //TODO
    //Add Logger
    public class CustomerService : ICustomerService, IDisposable
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerCreditService _customerCreditService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private const int MimimumAdultAge = 21;
        private const int MinimumCreditLimitThreshold = 500;

        public CustomerService(ICustomerCreditService customerCreditService, 
            ICompanyRepository companyRepository, 
            ICustomerRepository customerRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));             
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _customerCreditService = customerCreditService ?? throw new ArgumentNullException(nameof(customerCreditService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        [Obsolete]
        public CustomerService() : this(new CustomerCreditServiceClient(), 
            new CompanyRepository(), 
            new CustomerRepository(), 
            new DateTimeProvider())
        {
        }

        //TODO
        //Add Exception handling
        //Return new Customer instead of bool
        public bool AddCustomer(CustomerDto customerDto)
        {
            if (!IsValidCustomer(customerDto))
                return false;

            var company = _companyRepository.GetById(customerDto.CompanyId);

            var customer = new Customer
            {
                Company = company,
                DateOfBirth = customerDto.DateOfBirth,
                EmailAddress = customerDto.EmailAddress,
                Firstname = customerDto.Firstname,
                Surname = customerDto.Surname
            };

            SetCreditLimit(customer, company);

            if (IsCreditLimitBelowThreshold(customer))
            {
                return false;
            }

            _customerRepository.AddCustomer(customer);

            return true;
        }

        [Obsolete("Please use App.Services.CustomerService.AddCustomer(CustomerDto customerDto) instead")]
        public bool AddCustomer(string firname,
        string surname,
        string email,
        DateTime dateOfBirth,
        int companyId)
        {
            var customerDto = new CustomerDto
            {
                CompanyId = companyId,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            return AddCustomer(customerDto);
        }

        private bool IsValidCustomer(CustomerDto customerDto)
        {
            if (string.IsNullOrEmpty(customerDto.Firstname) || string.IsNullOrEmpty(customerDto.Surname))
            {
                return false;
            }

            if (!IsValidEmail(customerDto.EmailAddress))
            {
                return false;
            }

            if (!IsAdult(customerDto.DateOfBirth))
            {
                return false;
            }

            return true;
        }

        private bool IsCreditLimitBelowThreshold(Customer customer) => customer.HasCreditLimit && customer.CreditLimit < MinimumCreditLimitThreshold;

        private bool IsValidEmail(string email) => !string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains(".");

        private bool IsAdult(DateTime dateOfBirth) => MimimumAdultAge <= CalculateAge(dateOfBirth);
        
        //More enhancement can be done in this area
        private void SetCreditLimit(Customer customer, Company company)
        {
            var companyName = company?.Name;

            if (companyName == CompanyType.VeryImportantClient)
            {
                // Skip credit check
                customer.HasCreditLimit = false;
                return;
            }

            var creditLimit = _customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);

            if (companyName == CompanyType.ImportantClient)
            {
                // Do credit check and double credit limit                
                creditLimit = creditLimit * 2;                
            }

            customer.HasCreditLimit = true;
            customer.CreditLimit = creditLimit;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = _dateTimeProvider.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)
            {
                age--;
            }

            return age;
        }

        public void Dispose()
        {
            if (_customerCreditService is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}

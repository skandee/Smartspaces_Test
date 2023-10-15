using App.Constants;
using App.Dto;
using App.Entities;
using App.Repositories;
using Moq;
using NUnit.Framework;
using System;

namespace App.Tests.Services
{
    [TestFixture]
    internal class CustomerServiceTest
    {
        private ICustomerService _customerService;
        private Mock<ICompanyRepository> _companyRepositoryMock;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<ICustomerCreditService> _customerCreditServiceMock;
        private Mock<IDateTimeProvider> _dateTimeProviderMock;
        private const string FirstName = "John";
        private const string LastName = "Harry";
        private const string Email = "Joh.Harry@test.com";
        private const int CompanyId = 12;
        private DateTime AdultDateOfBirth = DateTime.Now.AddYears(-21);
        private CustomerDto _validCustomerDto;
                
        [SetUp]
        public void Setup()
        {
            SetupData();
            _companyRepositoryMock = new Mock<ICompanyRepository> ();
            _customerRepositoryMock = new Mock<ICustomerRepository> ();
            _customerCreditServiceMock = new Mock<ICustomerCreditService>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider> ();
            _dateTimeProviderMock.Setup(x => x.Now).Returns(DateTime.Now);
            _customerService = new CustomerService(_customerCreditServiceMock.Object, 
                _companyRepositoryMock.Object, 
                _customerRepositoryMock.Object, 
                _dateTimeProviderMock.Object);
        }

        private void SetupData()
        {
            _validCustomerDto = new CustomerDto
            {
                EmailAddress = Email,
                Firstname = FirstName,
                Surname = LastName,
                CompanyId = CompanyId,
                DateOfBirth = AdultDateOfBirth
            };
        }

        [Test]
        public void AddCustomer_WhenValidCustomer_And_VeryImportantCompany_ReturnTrue()
        {
            Customer customer = null;
            var company = new Company { Name = CompanyType.VeryImportantClient, Id = CompanyId };
            _companyRepositoryMock.Setup(x => x.GetById(CompanyId)).Returns(company);
            _customerRepositoryMock.Setup(x => x.AddCustomer(It.IsAny<Customer>())).Callback<Customer>(x => customer = x);

            var returnedResult = _customerService.AddCustomer(_validCustomerDto);

            Assert.IsTrue(returnedResult);
            Assert.IsFalse(customer.HasCreditLimit, "HasCreditLimit is not False");
            Assert.AreEqual(FirstName, customer.Firstname);
            Assert.AreEqual(LastName, customer.Surname);
            Assert.AreEqual(Email, customer.EmailAddress);
            Assert.AreEqual(AdultDateOfBirth, customer.DateOfBirth);
            Assert.AreEqual(CompanyId, customer.Company.Id);

            _customerCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        [TestCase(250)]
        [TestCase(251)]
        public void AddCustomer_WhenValidCustomer_And_ImportantCompany_And_CreditLimitAboveThreshold_ReturnTrue(int creditLimit)
        {
            Customer customer = null;
            var company = new Company { Name = CompanyType.ImportantClient };
            _companyRepositoryMock.Setup(x => x.GetById(CompanyId)).Returns(company);
            _customerRepositoryMock.Setup(x => x.AddCustomer(It.IsAny<Customer>())).Callback<Customer>(x => customer = x);
            _customerCreditServiceMock.Setup(x => x.GetCreditLimit(FirstName, LastName, AdultDateOfBirth)).Returns(creditLimit);

            var returnedResult = _customerService.AddCustomer(_validCustomerDto);

            Assert.IsTrue(returnedResult);
            Assert.IsTrue(customer.HasCreditLimit, "HasCreditLimit is not True");
            Assert.AreEqual(creditLimit * 2, customer.CreditLimit, "The CreditLimit is not correct");
        }

        [Test]
        [TestCase(249)]
        [TestCase(248)]
        public void AddCustomer_WhenValidCustomer_And_ImportantCompany_And_CreditLimitBelowThreshold_ReturnFalse(int creditLimit)
        {
            var company = new Company { Name = CompanyType.ImportantClient };
            _companyRepositoryMock.Setup(x => x.GetById(CompanyId)).Returns(company);
            _customerCreditServiceMock.Setup(x => x.GetCreditLimit(FirstName, LastName, AdultDateOfBirth)).Returns(creditLimit);

            var returnedResult = _customerService.AddCustomer(_validCustomerDto);

            Assert.IsFalse(returnedResult, "The returned result is not False");

            _customerRepositoryMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        [TestCase("", 500)]
        [TestCase(null, 500)]
        [TestCase("Smart", 500)]
        [TestCase("", 501)]
        [TestCase(null, 501)]
        [TestCase("Smart", 501)]
        public void AddCustomer_WhenValidCustomer_And_NotImportantCompany_And_CreditLimitAboveThreshold_ReturnTrue(string companyName, int creditLimit)
        {
            Customer customer = null;
            var company = new Company { Name = companyName };
            _companyRepositoryMock.Setup(x => x.GetById(CompanyId)).Returns(company);
            _customerRepositoryMock.Setup(x => x.AddCustomer(It.IsAny<Customer>())).Callback<Customer>(x => customer = x);
            _customerCreditServiceMock.Setup(x => x.GetCreditLimit(FirstName, LastName, AdultDateOfBirth)).Returns(creditLimit);

            var returnedResult = _customerService.AddCustomer(_validCustomerDto);

            Assert.IsTrue(returnedResult);
            Assert.IsTrue(customer.HasCreditLimit, "HasCreditLimit is not True");
            Assert.AreEqual(creditLimit, customer.CreditLimit, "The CreditLimit is not correct");
        }
        
        [Test]
        [TestCase("", 499)]
        [TestCase(null, 499)]
        [TestCase("Smart", 499)]        
        public void AddCustomer_WhenValidCustomer_And_NotImportantCompany_And_CreditLimitBelowThreshold_ReturnFalse(string companyName, int creditLimit)
        {
            var company = new Company { Name = companyName };
            _companyRepositoryMock.Setup(x => x.GetById(CompanyId)).Returns(company);
            _customerCreditServiceMock.Setup(x => x.GetCreditLimit(FirstName, LastName, AdultDateOfBirth)).Returns(creditLimit);

            var returnedResult = _customerService.AddCustomer(_validCustomerDto);

            Assert.IsFalse(returnedResult, "The returned result is not False");

            _customerRepositoryMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }


        [Test]
        [TestCase("", LastName, Email)]
        [TestCase(FirstName, "", Email)]
        [TestCase(FirstName, LastName, "")]
        [TestCase(FirstName, LastName, "john.harry")]
        [TestCase(FirstName, LastName, "johnharry@test")]
        [TestCase(null, LastName, Email)]
        [TestCase(FirstName, null, Email)]
        [TestCase(FirstName, LastName, null)]
        public void AddCustomer_WhenInValidCustomerFirstNameOrLastNameOrEmail__ReturnFalse(string firstName , string lastName, string email)
        {
            var customerDto = new CustomerDto
            {
                EmailAddress = email,
                Firstname = firstName,
                Surname = lastName,
                CompanyId = CompanyId,
                DateOfBirth = AdultDateOfBirth
            };
            var returnedResult = _customerService.AddCustomer(customerDto);

            Assert.IsFalse(returnedResult);

            _customerCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            _companyRepositoryMock.Verify(x => x.GetById(CompanyId),Times.Never);
            _customerRepositoryMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        [TestCase(20)]
        public void AddCustomer_WhenCustomerIsUnderAge__ReturnFalse(int age)
        {
            var dateOfBirth = DateTime.Now.AddYears(-age);

            var customerDto = new CustomerDto
            {
                EmailAddress = Email,
                Firstname = FirstName,
                Surname = LastName,
                CompanyId = CompanyId,
                DateOfBirth = dateOfBirth
            };

            var returnedResult = _customerService.AddCustomer(customerDto);

            Assert.IsFalse(returnedResult);

            _customerCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            _companyRepositoryMock.Verify(x => x.GetById(CompanyId), Times.Never);
            _customerRepositoryMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }
    }
}

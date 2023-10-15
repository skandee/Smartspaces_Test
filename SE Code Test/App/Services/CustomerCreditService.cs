using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    [System.CodeDom.Compiler.GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContract(ConfigurationName = "App.ICustomerCreditService")]
    public interface ICustomerCreditService
    {

        [System.ServiceModel.OperationContract(Action = "http://tempuri.org/ICustomerCreditService/GetCreditLimit", ReplyAction = "http://tempuri.org/ICustomerCreditService/GetCreditLimitResponse")]
        int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth);
    }

    [System.CodeDom.Compiler.GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface ICustomerCreditServiceChannel : ICustomerCreditService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThrough()]
    [System.CodeDom.Compiler.GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public partial class CustomerCreditServiceClient : System.ServiceModel.ClientBase<ICustomerCreditService>, ICustomerCreditService
    {

        public CustomerCreditServiceClient()
        {
        }

        public CustomerCreditServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public CustomerCreditServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public CustomerCreditServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public CustomerCreditServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth)
        {
            return Channel.GetCreditLimit(firstname, surname, dateOfBirth);
        }
    }
}

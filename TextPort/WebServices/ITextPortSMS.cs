using System.ServiceModel;

namespace TextPort.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITextPortSMS" in both code and config file together.
    [ServiceContract]
    public interface ITextPortSMS
    {
        [OperationContract]
        string Ping();

        [OperationContract]
        string VerifyAuthentication(string userName, string password);

        [OperationContract]
        int GetCreditBalance(string userName, string password);

        [OperationContract]
        TextPortSMSResponses SendMessages(TextPortSMSMessages messages);
    }
}

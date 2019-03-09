using System;
using System.Threading.Tasks;

using Amazon.SQS;
using Amazon.SQS.Model;

namespace TextPortCore.Integrations.AWS.SQS
{
    public class AWSSQS
    {
        public bool SendPosition(string latitude, string longitude)
        {
            AmazonSQSClient sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.USWest2);
            Amazon.SQS.Model.SendMessageRequest request = new Amazon.SQS.Model.SendMessageRequest();
            request.QueueUrl = "https://sqs.us-west-2.amazonaws.com/155879163045/TextPort.fifo";
            request.MessageBody = "lat: " + latitude + " lon: " + longitude;
            sqsClient.SendMessageAsync(request);
            return true;
        }
       
        public async Task<string> SendMessage(string message)
        {
            string retStr = string.Empty;
            try
            {
                AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig()
                {
                    ServiceURL = "http://sqs.us-west-2.amazonaws.com"
                };

                AmazonSQSClient sqsClient = new AmazonSQSClient(amazonSQSConfig);

                GetQueueUrlRequest request = new GetQueueUrlRequest()
                {
                    QueueName = "TextPort.fifo",
                    QueueOwnerAWSAccountId = "155879163045"
                };

                GetQueueUrlResponse response = await sqsClient.GetQueueUrlAsync(request);

                Console.WriteLine("Queue URL: " + response.QueueUrl);

                string foo = response.QueueUrl;

                SendMessageRequest sendMessageRequest = new SendMessageRequest()
                {
                    //MessageBody = $"{{message}}"
                    MessageBody = string.Format("{{0}}", message)
                };

                SendMessageResponse sendMessageResponse = await sqsClient.SendMessageAsync(sendMessageRequest);

                var bar = sendMessageResponse;

                retStr = sendMessageResponse.MessageId;
            }
            catch (Exception ex)
            {
                string expMsg = ex.Message;
            }

            return retStr;
        }

    }
}

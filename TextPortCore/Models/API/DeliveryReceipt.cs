namespace TextPortCore.Models.API
{
    public class DeliveryReceipt
    {
        public string From { get; set; }
        public string To { get; set; }
        public string MessageText { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public int MessageId { get; set; }

        public DeliveryReceipt()
        {
            this.From = string.Empty;
            this.To = string.Empty;
            this.MessageText = string.Empty;
            this.Status = string.Empty;
            this.ErrorMessage = string.Empty;
            this.MessageId = 0;
        }

        //public DeliveryReceipt()
        //{
        //    this.From = string.Empty;
        //    this.To = string.Empty;
        //    this.MessageText = string.Empty;
        //    this.Status = string.Empty;
        //    this.ErrorMessage = string.Empty;
        //    this.MessageId = 0;
        //}
    }
}

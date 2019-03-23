using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class MessageImage
    {
        private int accountId;
        private int imageId;
        private int messageId;
        private MessageDirection direction;
        private ImageStorageRepository storageRepository;
        private string fileName;
        private string url;

        public int AccountId
        {
            get { return this.accountId; }
            set { this.accountId = value; }
        }

        public int ImageId
        {
            get { return this.imageId; }
            set { this.imageId = value; }
        }

        public int MessageId
        {
            get { return this.messageId; }
            set { this.messageId = value; }
        }

        public MessageDirection Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        public ImageStorageRepository StorageRepository
        {
            get { return this.storageRepository; }
            set { this.storageRepository = value; }
        }

        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        public MessageImage()
        {
            this.accountId = 0;
            this.ImageId = 0;
            this.MessageId = 0;
            this.Direction = MessageDirection.Outbound;
            this.StorageRepository = ImageStorageRepository.Temporary;
            this.FileName = string.Empty;
            this.Url = string.Empty;
        }

        public MessageImage(int acctId, string fileName, MessageDirection msgDirection, ImageStorageRepository storageLocation)
        {
            this.AccountId = acctId;
            this.ImageId = RandomString.RandomNumber();
            this.MessageId = 0;
            this.Direction = msgDirection;
            this.StorageRepository = storageLocation;
            this.FileName = fileName;
            this.Url = getImageUrl(this.AccountId, this.FileName, this.StorageRepository);
        }

        private string getImageUrl(int accountId, string fileName, ImageStorageRepository storageLocation)
        {
            string baseUrl = string.Empty;
            string imageUrl = string.Empty;

            switch (storageLocation)
            {
                case ImageStorageRepository.Temporary:
                    baseUrl = ConfigurationManager.AppSettings["MMSImagesBaseUrl"];
                    imageUrl = $"{baseUrl}temp/{accountId}/{fileName}";
                    break;

                case ImageStorageRepository.Recent:
                case ImageStorageRepository.Archive:
                    baseUrl = ConfigurationManager.AppSettings["MMSImagesBaseUrl"];
                    imageUrl = $"{baseUrl}{accountId}/{fileName}";
                    break;
            }

            return imageUrl;
        }
    }
}

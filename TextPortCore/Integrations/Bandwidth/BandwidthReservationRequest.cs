using System;
using System.Collections.Generic;

using TextPortCore.Models;

namespace TextPortCore.Integrations.Bandwidth
{
    [Serializable()]
    public class Reservation
    {
        public string ReservedTn { get; set; }

        public Reservation()
        {
            this.ReservedTn = string.Empty;
        }

        public Reservation(string number)
        {
            this.ReservedTn = number;
        }
    }

    // Response
    public class ReservationResponse
    {
        public ResponseStatusItem ResponseStatus { get; set; }
    }

    public class ResponseStatusItem
    {
        public string ErrorCode { get; set; }
        public string Description { get; set; }
    }

}
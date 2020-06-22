using System;

namespace Flaky.Data
{
    public enum TranscationStatus {
        Pending,
        IncompleteInformation,
        BadInformation,
        Completed,
        Failed,
        Fraud
    }

    public class Transcation
    {
        public Guid ID { get; set; }

        public int ChargeID { get; set; }

        public string ReciptURL { get; set; }

        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public TranscationStatus Status { get; set; }
    }
}
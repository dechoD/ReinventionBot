using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Globalization;

namespace ReinventionBot.Models
{
    public class MergedWorkItemEntity : TableEntity
    {
        public MergedWorkItemEntity(string _, string rowKey)
            : base(_, rowKey)
        {
            // This will keep the records sorted my date
            this.PartitionKey = DateTime
                                    .MaxValue
                                    .Subtract(DateTime.UtcNow)
                                    .TotalMilliseconds
                                    .ToString(CultureInfo.InvariantCulture);
            this.RowKey = rowKey;
        }
    }
}
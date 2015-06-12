using System;

namespace Ica.StackIt.Application.Billing
{
	public class LineItem
	{
		// InvoiceID
		// PayerAccountId
		// LinkedAccountId
		// RecordType
		public string RecordType { get; set; }
		// RecordId
		public string RecordId { get; set; }
		// ProductName
		// RateId
		// SubscriptionId
		// PricingPlanId
		// UsageType
		// Operation
		// AvailabilityZone
		// ReservedInstance
		// ItemDescription
		// UsageStartDate
		public DateTime UsageStartDate { get; set; }
		// UsageEndDate
		public DateTime UsageEndDate { get; set; }
		// UsageQuantity
		// BlendedRate
		// BlendedCost
		public decimal BlendedCost { get; set; }
		// UnBlendedRate
		// UnBlendedCost
		// ResourceId
		public string ResourceId { get; set; }
		// user:Environment
		// user:Owner
		// user:Servername
		// user:owner
	}
}
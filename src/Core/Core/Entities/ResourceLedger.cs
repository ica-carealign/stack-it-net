using System;
using System.Collections.Generic;
using System.Linq;

namespace Ica.StackIt.Core.Entities
{
	public class ResourceLedger : IEntity<Guid>
	{
		public ResourceLedger()
		{
			Periods = new Dictionary<string, Dictionary<string, decimal>>();
		}

		// todo: revamp repository layer to support non-guid entities, then use resourceid as the id
		public Guid Id { get; set; }

		// todo: index resourceid
		public string ResourceId { get; set; }

		public IDictionary<string, Dictionary<string, decimal>> Periods { get; set; }

		public decimal TotalCost
		{
			get { return CalculateTotalCost(); }
			// ReSharper disable once ValueParameterNotUsed
			// Cause the storage serializer to include TotalCost in the database but to ignore it when loading.
			set { }
		}

		public decimal CalculateTotalCost()
		{
			return Periods.Values.Select(x => x.Values.Sum()).Sum();
		}

		public IDictionary<string, decimal> GetPeriod(string period)
		{
			if (Periods.ContainsKey(period))
			{
				return Periods[period];
			}
			return Periods[period] = new Dictionary<string, decimal>();
		}
	}
}
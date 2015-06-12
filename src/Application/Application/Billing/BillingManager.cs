using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Billing
{
	public class BillingManager : IBillingManager
	{
		private readonly IRepository<ResourceLedger> _resourceLedgerRepository;

		public BillingManager(IRepository<ResourceLedger> resourceLedgerRepository)
		{
			_resourceLedgerRepository = resourceLedgerRepository;
		}

		public void WipeAllData()
		{
			_resourceLedgerRepository.DeleteAll();
		}

		public void LoadLineItems(IEnumerable<LineItem> lineItems, string period)
		{
			var sw = new Stopwatch();
			sw.Start();
			int records = 0;
			foreach (IGrouping<string, LineItem> group in lineItems.GroupBy(li => li.ResourceId))
			{
				ResourceLedger record = FindOrCreateLedger(group.Key);
				foreach (LineItem lineItem in group)
				{
					records++;
					record.GetPeriod(period)[lineItem.RecordId] = lineItem.BlendedCost;
				}
				_resourceLedgerRepository.Update(record);
			}
			sw.Stop();
			Debug.WriteLine("Processed {0} records in {1:0.00} seconds ({2:0.000}/sec)",
				records, sw.Elapsed.TotalSeconds, records/sw.Elapsed.TotalSeconds);
		}

		private ResourceLedger FindOrCreateLedger(string resourceId)
		{
			ResourceLedger record = FindLedger(resourceId);
			if (record == null)
			{
				record = new ResourceLedger {Id = Guid.NewGuid(), ResourceId = resourceId};
				_resourceLedgerRepository.Add(record);
			}
			return record;
		}

		private ResourceLedger FindLedger(string resourceId)
		{
			return _resourceLedgerRepository.CreateQuery()
			                                .FirstOrDefault(db => db.ResourceId == resourceId);
		}
	}
}
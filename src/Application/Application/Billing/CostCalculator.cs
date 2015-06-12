using System.Linq;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Billing
{
	public class CostCalculator
	{
		private readonly IRepository<Instance> _instanceRepository;
		private readonly IRepository<ResourceLedger> _ledgerRepository;

		public CostCalculator(
			IRepository<Instance> instanceRepository,
			IRepository<ResourceLedger> ledgerRepository
			)
		{
			_instanceRepository = instanceRepository;
			_ledgerRepository = ledgerRepository;
		}

		public decimal CalculateCost(Stack stack)
		{
			return stack.InstanceIds
			            .Sum(id => CalculateCost(_instanceRepository.Find(id)));
		}

		public decimal CalculateCost(Instance instance)
		{
			if (instance == null)
			{
				return 0;
			}

			return new[] {instance.ResourceId}.Concat(instance.VolumeResourceIds)
			                                  .Select(GetLedgerByResourceId)
			                                  .Where(ledger => ledger != null)
			                                  .Sum(ledger => ledger.TotalCost);
		}

		private ResourceLedger GetLedgerByResourceId(string id)
		{
			return _ledgerRepository.CreateQuery().FirstOrDefault(l => l.ResourceId == id);
		}
	}
}
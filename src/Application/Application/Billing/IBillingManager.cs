using System.Collections.Generic;

namespace Ica.StackIt.Application.Billing
{
	public interface IBillingManager
	{
		void WipeAllData();

		void LoadLineItems(IEnumerable<LineItem> lineItems, string period);
	}
}
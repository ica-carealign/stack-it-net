using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core
{
	public interface IEntity<out TId>
	{
		[Required]
		TId Id { get; }
	}
}
using Microsoft.Extensions.Primitives;

namespace Infrastructure.AI.Tests.Helpers;

public class NoChangeToken : IChangeToken
{
	public bool HasChanged => false;
	public bool ActiveChangeCallbacks => false;
	public IDisposable RegisterChangeCallback(Action<object> callback, object state) => new NoOpDisposable();

	private sealed class NoOpDisposable : IDisposable
	{
		public void Dispose() { }
	}
}

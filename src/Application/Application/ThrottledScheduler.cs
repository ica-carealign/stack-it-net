using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ica.StackIt.Application
{
	public sealed class ThrottledScheduler : IDisposable
	{
		private readonly TimeSpan _atLeastOnceEvery;
		private readonly TimeSpan _atMostOncePer;
		private readonly Action _periodicAction;
		private readonly object _sync = new object();

		private Timer _timer;
		private DateTime _nextRun;
		private DateTime _nextAllowedRun;

		/// <summary>
		///     Schedules <paramref name="periodicAction" /> to be called at least once for every
		///     <paramref name="atLeastOnceEvery" /> span, but throttles to ensure it is only called at most during
		///     any <paramref name="atMostOncePer" /> span.
		/// </summary>
		/// <param name="atLeastOnceEvery">
		///     The maximum amount of time that should pass before the <paramref name="periodicAction" />
		///     is called at least once.
		/// </param>
		/// <param name="atMostOncePer">The limit to how often the <paramref name="periodicAction" /> should be called.</param>
		/// <param name="periodicAction">The code to run on schedule. <b>Must be reentrant.</b></param>
		public ThrottledScheduler(TimeSpan atLeastOnceEvery, TimeSpan atMostOncePer, Action periodicAction)
		{
			if (atMostOncePer > atLeastOnceEvery)
			{
				throw new ArgumentException("atMostOncePer must be less than or equal to atLeastOnceEvery", "atMostOncePer");
			}
			_atLeastOnceEvery = atLeastOnceEvery;
			_atMostOncePer = atMostOncePer;
			_periodicAction = periodicAction;
		}

		public void Trigger()
		{
			lock (_sync)
			{
				if (_timer == null)
				{
					return;
				}

				DateTime now = DateTime.UtcNow;
				if (now >= _nextAllowedRun)
				{
					RunAt(now, now);
				}
				else if (_nextRun > _nextAllowedRun)
				{
					RunAt(_nextAllowedRun, now);
				}
				// else already scheduled to run as early as possible
			}
		}

		private void RunAt(DateTime when, DateTime now)
		{
			TimeSpan interval = when - now;
			if (interval < TimeSpan.Zero)
			{
				interval = TimeSpan.Zero;
			}
			_timer.Change(interval, _atLeastOnceEvery);
		}

		private void OnTimer(object state)
		{
			lock (_sync)
			{
				Task.Factory.StartNew(_periodicAction);
				DateTime now = DateTime.UtcNow;
				_nextAllowedRun = now + _atMostOncePer;
				_nextRun = now + _atLeastOnceEvery;
				RunAt(_nextRun, now);
			}
		}

		/// <summary>
		///     Starts the scheduler. Throws an exception if already started.
		/// </summary>
		public void Start()
		{
			lock (_sync)
			{
				if (_timer != null)
				{
					throw new InvalidOperationException("already started");
				}
				DateTime now = DateTime.UtcNow;
				_nextAllowedRun = now;
				_nextRun = now + _atLeastOnceEvery;
				_timer = new Timer(OnTimer);
				RunAt(_nextRun, now);
			}
		}

		/// <summary>
		///     Stops the scheduler if it is running.
		/// </summary>
		public void Stop()
		{
			lock (_sync)
			{
				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}
}
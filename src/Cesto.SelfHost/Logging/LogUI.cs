using System;
using Cesto.WinForms;

namespace Cesto.Logging
{
	/// <summary>
	/// Contains a simple facility for saving logging events that will be displayed
	/// on a UI in the self-hosted program.
	/// </summary>
	public static class LogUI
	{
		/// <summary>
		/// Log level, or severity. Each different level is displayed with different icons and/or colours.
		/// </summary>
		public enum Level
		{
			Debug,
			Info,
			Warning,
			Error,
			Fatal
		}

		/// <summary>
		/// Simple representation of a single log event that will be displayed in the UI.
		/// </summary>
		public class Event
		{
			public DateTime Timestamp { get; set; }
			public Level Level { get; set; }
			public string Logger { get; set; }
			public string Message { get; set; }
		}

		private static readonly EventLoggingDataSource<Event> EventLoggingDataSource;

		static LogUI()
		{
			EventLoggingDataSource = new EventLoggingDataSource<Event> {
				IsDebugCallback = IsDebug
			};
		}

		public static IVirtualDataSource<Event> DataSource
		{
			get
			{
				return EventLoggingDataSource;
				;
			}
		}

		public static void AddEvent(Event ev)
		{
			EventLoggingDataSource.Add(ev);
		}

		private static bool IsDebug(Event ev)
		{
			return ev.Level == Level.Debug;
		}
	}
}

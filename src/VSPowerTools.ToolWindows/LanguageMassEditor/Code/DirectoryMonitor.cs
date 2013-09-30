using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.Code
{
	public delegate void FileSystemEvent(String path);

	public interface IDirectoryMonitor
	{
		event FileSystemEvent Change;
		void Start();
	}

	public class DirectoryMonitor : IDirectoryMonitor
	{
		private readonly FileSystemWatcher _watcher =
			new FileSystemWatcher();
		private readonly Dictionary<string, DateTime> _pendingEvents =
			new Dictionary<string, DateTime>();
		private readonly Timer _timer;
		private bool _timerStarted = false;

		public DirectoryMonitor(string dirPath)
		{
			_watcher.Path = dirPath;
			_watcher.IncludeSubdirectories = false;
			_watcher.Created += new FileSystemEventHandler(OnChange);
			_watcher.Changed += new FileSystemEventHandler(OnChange);

			_timer = new Timer(OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
		}

		public event FileSystemEvent Change;

		public void Start()
		{
			_watcher.EnableRaisingEvents = true;
		}

		private void OnChange(object sender, FileSystemEventArgs e)
		{
			// Don't want other threads messing with the pending events right now
			lock (_pendingEvents)
			{
				// Save a timestamp for the most recent event for this path
				_pendingEvents[e.FullPath] = DateTime.Now;

				// Start a timer if not already started
				if (!_timerStarted)
				{
					_timer.Change(100, 100);
					_timerStarted = true;
				}
			}
		}

		private void OnTimeout(object state)
		{
			List<string> paths;

			// Don't want other threads messing with the pending events right now
			lock (_pendingEvents)
			{
				// Get a list of all paths that should have events thrown
				paths = FindReadyPaths(_pendingEvents);

				// Remove paths that are going to be used now
				paths.ForEach(delegate(string path)
				{
					_pendingEvents.Remove(path);
				});

				// Stop the timer if there are no more events pending
				if (_pendingEvents.Count == 0)
				{
					_timer.Change(Timeout.Infinite, Timeout.Infinite);
					_timerStarted = false;
				}
			}

			// Fire an event for each path that has changed
			paths.ForEach(delegate(string path)
			{
				FireEvent(path);
			});
		}

		private List<string> FindReadyPaths(Dictionary<string, DateTime> events)
		{
			List<string> results = new List<string>();
			DateTime now = DateTime.Now;

			foreach (KeyValuePair<string, DateTime> entry in events)
			{
				// If the path has not received a new event in the last 75ms
				// an event for the path should be fired
				double diff = now.Subtract(entry.Value).TotalMilliseconds;
				if (diff >= 75)
				{
					results.Add(entry.Key);
				}
			}

			return results;
		}

		private void FireEvent(string path)
		{
			FileSystemEvent evt = Change;
			if (evt != null)
			{
				evt(path);
			}
		}
	}
}

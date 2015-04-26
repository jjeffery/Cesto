using System;
using System.IO;

namespace Cesto
{
	/// <summary>
	/// Common class for access work directory locations.
	/// </summary>
	public static class WorkDirectory
	{
		/// <summary>
		/// The base directory under which all other work directories are located.
		/// Default location is under %PROGRAMDATA%.
		/// </summary>
		public static string Base;

		/// <summary>
		/// Event that is raised whenever a work directory is created. This allows
		/// the calling program to hook up logging if desired.
		/// </summary>
		public static event Action<string> DirectoryCreated;

		/// <summary>
		/// Directory for log files.
		/// </summary>
		public static string Log
		{
			get { return For("log"); }
		}

		/// <summary>
		/// Directory for config files.
		/// </summary>
		public static string Config
		{
			get { return For("config"); }
		}

		/// <summary>
		/// Directory for database and data files.
		/// </summary>
		public static string Data
		{
			get { return For("data"); }
		}

		/// <summary>
		/// Return path for work directory with the given name. The work
		/// directory is created if it does not already exist.
		/// </summary>
		/// <param name="name">Name of the work directory.</param>
		/// <returns>
		/// Full path for work directory.
		/// </returns>
		public static string For(string name)
		{
			InitializeIfNecessary();
			var path = Path.Combine(Base, name);
			CheckDirectory(path);
			return path;
		}

		private static void InitializeIfNecessary()
		{
			if (Base == null)
			{
				var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				CheckDirectory(programData, true);
				var companyDirectory = Path.Combine(programData, ApplicationInfo.CompanyName);
				CheckDirectory(companyDirectory);
				Base = Path.Combine(companyDirectory, ApplicationInfo.ProductName);
				CheckDirectory(Base);
			}
		}

		private static void CheckDirectory(string path, bool mustExist = false)
		{
			if (!Directory.Exists(path))
			{
				if (mustExist)
				{
					var message = string.Format("Missing directory: {0}", path);
					throw new CestoException(message);
				}

				try
				{
					Directory.CreateDirectory(path);
					var action = DirectoryCreated;
					if (action != null)
					{
						action(path);
					}
				}
				catch (Exception ex)
				{
					var message = string.Format("Cannot create directory {0}: {1}", path, ex.Message);
					throw new CestoException(message, ex);
				}
			}
		}
	}
}

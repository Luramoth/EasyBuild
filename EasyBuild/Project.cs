///This Source Code Form is subject to the terms of the Mozilla Public
///License, v. 2.0. If a copy of the MPL was not distributed with this
///file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;
using System.Text.Json;

namespace EasyBuild
{
	enum BuildType
	{
		Debug,
		Release
	}

	internal class Project
	{
		public string? ProjectDir;

		public BuildType BuildType;

		private string? BuildFile;

		private string[]? files;

		public int Build()
		{
			if (!PreBuild())
			{
				Console.WriteLine("Prebuild Failed, exiting...");
				return 1;
			}

			return 0;
		}

		public int Test()
		{
			return 0;
		}

		private bool PreBuild()
		{
			Console.WriteLine("Starting PreBuild...");

			try
			{
				ProjectDir = Path.GetFullPath(ProjectDir!);

				Directory.SetCurrentDirectory(ProjectDir!);

				files = Directory.GetFiles(ProjectDir!);
			}
			catch
			{
				Console.WriteLine("invalid project directory: " + ProjectDir!);
				return false;
			}

			foreach (string file in files)
			{
				string filename = Path.GetFileName(file);

				if (filename.Equals("easybuild.json", StringComparison.CurrentCultureIgnoreCase))
				{
					BuildFile = file;

					break;
				}
			}

			if (BuildFile is null)
			{
				Console.WriteLine("easybuild.json not found in project directory: " + ProjectDir!);
				return false;
			}



			return true;
		}
	}
}

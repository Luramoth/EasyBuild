///This Source Code Form is subject to the terms of the Mozilla Public
///License, v. 2.0. If a copy of the MPL was not distributed with this
///file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;

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

		public int Build()
		{
			string[] files;

			try
			{
				ProjectDir = Path.GetFullPath(ProjectDir!);

				Directory.SetCurrentDirectory(ProjectDir!);

				files = Directory.GetFiles(ProjectDir!);
			}
			catch
			{
				Console.WriteLine("PreBuild failed, invalid project directory: " + Path.GetFullPath(ProjectDir!));
				return 1;
			}

			foreach (string file in files)
			{
				if (file.Equals("easybuild.json", StringComparison.CurrentCultureIgnoreCase))
				{
					BuildFile = file;
				}
			}

			if (BuildFile is null)
			{
				Console.WriteLine("PreBuild failed, easybuild.json not found in project directory: " + ProjectDir!);
				return 1;
			}

			return 0;
		}

		public int Test()
		{
			return 0;
		}
	}
}

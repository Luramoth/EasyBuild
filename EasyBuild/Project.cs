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
				if (file.ToLower() == "build.easybuild")
				{
					BuildFile = file;
				}
			}

			if (BuildFile is null)
			{
				Console.WriteLine("PreBuild failed, build.easybuild not found in project directory: " + ProjectDir!);
				return 1;
			}

			return 0;
		}
	}
}

///This Source Code Form is subject to the terms of the Mozilla Public
///License, v. 2.0. If a copy of the MPL was not distributed with this
///file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;
using System.Diagnostics;
using System.Text.Json.Nodes;

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

		private string? executible;

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
			ProcessStartInfo start = new();
			start.Arguments = "";
			start.FileName = executible!;
			start.WindowStyle = ProcessWindowStyle.Normal;
			start.CreateNoWindow = false;
			int exitCode;

			Process? activeProcess = Process.Start(start);
			using Process process = activeProcess!;
			process!.WaitForExit();
			exitCode = process.ExitCode;

			return 0;
		}

		private bool PreBuild()
		{
			Console.WriteLine("Starting PreBuild...");

			// see if project root is an actual directory
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

			// see if thers a easybuild.json file in project root
			foreach (string file in files)
			{
				string filename = Path.GetFileName(file);

				if (filename.Equals("easybuild.json", StringComparison.CurrentCultureIgnoreCase))
				{
					BuildFile = File.ReadAllText(file);

					break;
				}
			}

			if (BuildFile is null)
			{
				Console.WriteLine("easybuild.json not found in project directory: " + ProjectDir!);
				return false;
			}


			// Check is version is valid
			if ((float?)JsonNode.Parse(BuildFile)["EBversion"] < Program.Version)
			{
				Console.WriteLine("Newer version of EasyBuild required, Please update");
				Console.WriteLine("Hint: your project doesent always need the latest version of EasyBuild! lower the version if you dont use the latest features");

				return false;

			} else if ((float?)JsonNode.Parse(BuildFile)["EBversion"] is null)
			{
				Console.WriteLine("EasyBuild required version not found in easybuild.json, Terminating...");
				Console.WriteLine("Hint: Add {\"EBversion\": " + Program.Version + "} to the root of your easybuild.json file!");

				return false;
			} else
			{
				Console.WriteLine("EasyBuild Version Valid, Continuing...");
			}

			// Get project name
			if ((string?)JsonNode.Parse(BuildFile)["Name"] is null)
			{
				Console.WriteLine("Project name not found in easybuild.json, Terminating...");
				Console.WriteLine("Hint: Dont be modest! Add {\"Name\": \"" + Path.GetFileName(ProjectDir) + "\"} to the root of your easybuild.json file!");

				return false;
			}

			return true;
		}
	}
}

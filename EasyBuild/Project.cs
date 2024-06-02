///This Source Code Form is subject to the terms of the Mozilla Public
///License, v. 2.0. If a copy of the MPL was not distributed with this
///file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Text.Json.Nodes;
using log4net;
using log4net.Config;

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
		public string? ProjectName;
		public BuildType BuildType;
		public static readonly ILog log = LogManager.GetLogger(typeof(Project));

		private string? BuildFile;
		private string[]? files;
		private string? executible;
		private string? sourceDirectory;
		private string[]? sourceFiles;


		public Project()
		{
			XmlConfigurator.Configure(new FileInfo("LoggerConfig.xml"));
		}

		public int Build()
		{
			if (!PreBuild())
			{
				log.Fatal("Prebuild Failed, exiting...");
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
			log.Info("Starting PreBuild...");

			// see if project root is an actual directory
			try
			{
				ProjectDir = Path.GetFullPath(ProjectDir!);

				Directory.SetCurrentDirectory(ProjectDir!);

				files = Directory.GetFiles(ProjectDir!);
			}
			catch
			{
				log.Error("invalid project directory: " + ProjectDir!);
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
				log.Error("easybuild.json not found in project directory: " + ProjectDir!);
				return false;
			}


			// Check is version is valid
			if ((float?)JsonNode.Parse(BuildFile)["EBversion"] < Program.Version)
			{
				log.Error("Newer version of EasyBuild required, Please update");
				log.Info("Hint: your project doesent always need the latest version of EasyBuild! lower the version if you dont use the latest features");

				return false;

			} else if ((float?)JsonNode.Parse(BuildFile)["EBversion"] is null)
			{
				log.Error("EasyBuild required version not found in easybuild.json, Terminating...");
				log.Info("Hint: Add {\"EBversion\": " + Program.Version + "} to the root of your easybuild.json file!");

				return false;
			} else
			{
				log.Info("EasyBuild Version Valid, Continuing...");
			}

			// Get project name

			ProjectName = (string?)JsonNode.Parse(BuildFile)["Name"];

			if (ProjectName is null)
			{
				log.Error("Project name not found in easybuild.json, Terminating...");
				log.Info("Hint: Dont be modest! Add {\"Name\": \"" + Path.GetFileName(ProjectDir) + "\"} to the root of your easybuild.json file!");

				return false;
			} else if (ProjectName != ProjectName.ToLower())
			{
				log.Warn("Project name valid! Fully lowercase name reccomended");
			}

			// Get source files
			string? srcDirStr = (string?)JsonNode.Parse(BuildFile)["SourceDir"];
			if (srcDirStr is null)
			{
				log.Error("Source directory not found in easybuild.json, Terminating...");
				log.Info("Hint: Add {\"SourceDir\": \"./src/\"} to the root of your easybuild.json file!");

				return false;
			}
			else
			{
				sourceDirectory = Path.GetFullPath(srcDirStr!);

				try
				{
					sourceFiles = Directory.GetFiles(sourceDirectory);
				} catch
				{
					log.Error("Source directory invalid, Terminating...");

					return false;
				}

				if (sourceFiles.Length == 0)
				{
					log.Error("Source directory empty, Terminating...");
					return false;
				}
				else
				{
					foreach (string file in sourceFiles)
					{
						if (Path.GetFileName(file).Equals("easybuild.json", StringComparison.CurrentCultureIgnoreCase))
						{
							log.Warn("Source directory valid, please keep seperate from project root!");
							log.Info("Hint: put source files inside a folder like [./src/]");
						}
					}
				}
			}

			return true;
		}
	}
}

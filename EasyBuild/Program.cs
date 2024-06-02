///This Source Code Form is subject to the terms of the Mozilla Public
///License, v. 2.0. If a copy of the MPL was not distributed with this
///file, You can obtain one at http://mozilla.org/MPL/2.0/.

using EasyBuild;
using System.IO;

static class Program
{
	enum ArgMode
	{
		None,
		ProjPath
	}

	static ArgMode argMode = ArgMode.None;
	static readonly Project project = new();

	public const float Version = 6.24f;

	static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("Error, no arguments given, use -h for help");
			Console.WriteLine("Usage: easybuild <build|test> [debug|release] [-p <path>] \n");

			return;
		}

		if (args[0].Equals("build", StringComparison.CurrentCultureIgnoreCase))
		{
			HandleArgs(args.Skip(1).ToArray());

			project.Build();
		}
		else if (args[0].Equals("test", StringComparison.CurrentCultureIgnoreCase))
		{
			HandleArgs(args.Skip(1).ToArray());

			project.Build();
			project.Test();
		}
		else if (args[0].Equals("-v", StringComparison.CurrentCultureIgnoreCase))
		{
			PrintVersion();
		}
	}

	static private void HandleArgs(string[] args)
	{
		foreach (string arg in args)
		{
			switch (arg.ToLower())
			{
				case "debug":
					project.BuildType = BuildType.Debug;
					continue;

				case "release":
					project.BuildType = BuildType.Release;
					continue;

				case "-p":
					argMode = ArgMode.ProjPath;
					continue;

				default:
					switch (argMode)
					{
						case ArgMode.ProjPath:
							project.ProjectDir = arg;
							argMode = ArgMode.None;
							continue;
					}
					continue;
			}
		}
	}

	static private void PrintVersion()
	{
		Console.WriteLine("\nEasyBuild version " + Version);
		Console.WriteLine("A program to make building your projects easy\n");
		Console.WriteLine("This program is Liscenced under the Mozilla Public License, v. 2.0");
		Console.WriteLine("If the MPL was not distributed with your copy of EasyBuild, you can obtain one at http://mozilla.org/MPL/2.0/.\n");
	}
}
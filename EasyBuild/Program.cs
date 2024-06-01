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

	static void Main(string[] args)
	{
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
}
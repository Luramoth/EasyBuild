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

    static void Main(string[] args)
	{
		Project project = new Project();

		if (args[0].ToLower() == "build")
		{
			foreach (string arg in args)
			{
				switch (arg.ToLower()) {
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

			project.Build();
		}
	}
}
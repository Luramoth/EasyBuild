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
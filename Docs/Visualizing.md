For Visualizing of EF Model we can use VisualStruio extension called EFCorePowerTools.
We can install this tool through extensions.
DGML editor individual component in Visual Studio must be installed.
Data project must target runtime environment(such as netcoreapp3.1).
We also need to add "Microsoft.EntityFrameworkCore.SqlServer" Nuget package.

To see Model diagram
1. Right click Data project
2. Choose "EF Core Power Tools"
3. Selected "Add DbContext Model Diagram"
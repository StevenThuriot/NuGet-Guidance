NuGet Guidance [![Build status](https://ci.appveyor.com/api/projects/status?id=nivtc4h9ybci9yu6)](https://ci.appveyor.com/project/Nuget_Guidance)
====

Sometimes a NuGet package can contain complex install logic.

When this is the case, it can be a real hassle for a C# programmer to write all the code in powershell. 
This project is a hoster for any "recipe" you include in your nuget package. 

Using MEF, the recipes will run inside this hoster project.

For your convenience, all you have to do is inherit BaseRecipe when creating new recipes.

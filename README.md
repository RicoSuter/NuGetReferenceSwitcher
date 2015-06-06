# NuGet Reference Switcher

NuGet Reference Switcher is a Visual Studio extension which automatically switches NuGet assembly references to project references and vice-versa. This is useful when developing applications which reference own NuGet packages: When developing an application, switch to project references so that all code is editable and debuggable. After finishing the development, create new NuGet package versions, switch back to NuGet references and upgrade to the new NuGet versions. 

Features: 

- Switches from NuGet DLL references to project references and stores the changes to revert them later
- Automatically adds and removes projects to the solution if desired
- Checks project dependencies so that the projects get added and removed in the correct order

(This project has originally been hosted on [CodePlex](http://nugetreferenceswitcher.codeplex.com))

## Download from Visual Studio Gallery

* [Download for Visual Studio 2012](https://visualstudiogallery.msdn.microsoft.com/9a7bbfb3-1b44-4a59-8204-0a01abbeeaca)
* [Download for Visual Studio 2013](https://visualstudiogallery.msdn.microsoft.com/68878c27-110c-43ec-ae61-3ea3f7aae88c)
* [Download for Visual Studio 2015](https://visualstudiogallery.msdn.microsoft.com/e2458c0b-03c0-47a9-a94b-0d28567e0a84)

## [Guide about how to use the extension](https://github.com/rsuter/NuGetReferenceSwitcher/wiki/Guide)

**This is a fully working beta version, please provide feedback to improve the extension.**

![](http://rsuter.com/Upload/NuGetReferenceSwitcher/02.png)

The NuGet Reference Switcher extension is developed by [Rico Suter](http://rsuter.com) using the [MyToolkit](https://github.com/MyToolkit/Core) library. 


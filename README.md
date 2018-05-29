# NuGet Reference Switcher

NuGet Reference Switcher is a Visual Studio extension which automatically switches NuGet assembly references to project references and vice-versa. This is useful when developing applications which reference own NuGet packages: When developing an application, switch to project references so that all code is editable and debuggable. After finishing the development, create new NuGet package versions, switch back to NuGet references and upgrade to the new NuGet versions. 

## **[For .NET Core and Standard projects, check out DNT (DotNetTools)](https://github.com/RSuter/DNT)**

**[Guide about how to use the extension](https://github.com/rsuter/NuGetReferenceSwitcher/wiki/Guide)**

Features: 

- Switches from NuGet DLL references to project references and stores the changes to revert them later
- Automatically adds and removes projects to the solution if desired
- Checks project dependencies so that the projects get added and removed in the correct order

![](http://rsuter.com/Upload/NuGetReferenceSwitcher/02.png)

## Download from Visual Studio Gallery

* [Download for Visual Studio 2012](https://visualstudiogallery.msdn.microsoft.com/9a7bbfb3-1b44-4a59-8204-0a01abbeeaca)
* [Download for Visual Studio 2013](https://visualstudiogallery.msdn.microsoft.com/68878c27-110c-43ec-ae61-3ea3f7aae88c)
* [Download for Visual Studio 2015](https://visualstudiogallery.msdn.microsoft.com/e2458c0b-03c0-47a9-a94b-0d28567e0a84)
* [Download for Visual Studio 2017](https://marketplace.visualstudio.com/vsgallery/ef35a0f9-2712-4634-97e0-51c03f4be1e3)

The NuGet Reference Switcher extension is developed by [Rico Suter](http://rsuter.com) using the [MyToolkit](http://mytoolkit.io) library. 

(This project has originally been hosted on [CodePlex](http://nugetreferenceswitcher.codeplex.com))

## How to build the extension

In order to build the extension, you need to install the Visual Studio SDK(s): 

Visual Studio 2012 SDK: 
	
http://www.microsoft.com/en-us/download/details.aspx?id=30668

Visual Studio 2013 SDK: 

http://www.microsoft.com/en-us/download/details.aspx?id=40758
	
Visual Studio 2015 SDK: 

https://msdn.microsoft.com/en-us/library/mt683786.aspx

After installing the SDK, run one of the batch files from the `/build` directory in the corresponding Visual Studio Command Prompt. The release output can be found in the `/build/Output` directory. 

## How to debug the extension

Right click on the extension project and select `Properties`. In the `Debug` tab set `Start External Program` to one of the following Visual Studio executables: 

Visual Studio 2012: 

    C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe

Visual Studio 2013: 

    C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe

Visual Studio 2015: 

    C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe

Visual Studio 2017: 

    C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe

And set the `Command line arguments` to:

    /rootsuffix Exp


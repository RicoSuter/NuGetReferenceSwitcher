nuget restore ../src/NuGetReferenceSwitcher.VS17.sln
msbuild ../src/NuGetReferenceSwitcher.VS17.sln /p:Configuration=Release /t:rebuild

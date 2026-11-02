nuget restore ../src/NuGetReferenceSwitcher.VS26.sln
msbuild ../src/NuGetReferenceSwitcher.VS26.sln /p:Configuration=Release /t:rebuild

nuget restore ../src/NuGetReferenceSwitcher.VS13.sln
msbuild ../src/NuGetReferenceSwitcher.VS13.sln /p:Configuration=Release /t:rebuild

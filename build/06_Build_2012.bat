nuget restore ../src/NuGetReferenceSwitcher.VS12.sln
msbuild ../src/NuGetReferenceSwitcher.VS12.sln /p:Configuration=Release /t:rebuild

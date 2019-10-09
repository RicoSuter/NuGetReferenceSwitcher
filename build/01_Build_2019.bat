nuget restore ../src/NuGetReferenceSwitcher.VS19.sln
msbuild ../src/NuGetReferenceSwitcher.VS19.sln /p:Configuration=Release /t:rebuild

nuget restore ../src/NuGetReferenceSwitcher.VS15.sln
msbuild ../src/NuGetReferenceSwitcher.VS15.sln /p:Configuration=Release /t:rebuild

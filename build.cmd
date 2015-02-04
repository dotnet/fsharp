@echo off
rem Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information. -->

echo %~dp0
pushd %~dp0

".nuget/NuGet.exe" install NUnit -OutputDirectory packages -version 2.6.3
".nuget/NuGet.exe" install FAKE  -OutputDirectory packages -ExcludeVersion -version 3.14.6
packages\FAKE\tools\FAKE.exe build.fsx Default %*
popd

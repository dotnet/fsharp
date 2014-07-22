#!/bin/bash
if test "$OS" = "Windows_NT"
then
  # use .Net

  .nuget/NuGet.exe install NUnit -OutputDirectory packages -version 2.6.3
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi
  
  .nuget/NuGet.exe install FAKE  -OutputDirectory packages -ExcludeVersion -version 3.14.6
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  packages/FAKE/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx 
else
  # use mono
  mono .nuget/NuGet.exe install NUnit -OutputDirectory packages -version 2.6.3
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  mono .nuget/NuGet.exe install FAKE  -OutputDirectory packages -ExcludeVersion -version 3.14.6
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi
  mono packages/FAKE/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx 
fi

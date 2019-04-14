#!/usr/bin/env bash
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

# Stop script if unbound variable found (use ${var:-} if intentional)
set -u

usage()
{
  echo "Common settings:"
  echo "  --configuration <value>    Build configuration: 'Debug' or 'Release' (short: -c)"
  echo "  --verbosity <value>        Msbuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic] (short: -v)"
  echo "  --binaryLog                Create MSBuild binary log (short: -bl)"
  echo ""
  echo "Actions:"
  echo "  --bootstrap                Force the build of the bootstrap compiler"
  echo "  --restore                  Restore projects required to build (short: -r)"
  echo "  --norestore                Don't restore projects required to build"
  echo "  --build                    Build all projects (short: -b)"
  echo "  --rebuild                  Rebuild all projects"
  echo "  --pack                     Build nuget packages"
  echo "  --publish                  Publish build artifacts"
  echo "  --help                     Print help and exit"
  echo ""
  echo "Test actions:"
  echo "  --testcoreclr              Run unit tests on .NET Core (short: --test, -t)"
  echo ""
  echo "Advanced settings:"
  echo "  --ci                       Building in CI"
  echo "  --docker                   Run in a docker container if applicable"
  echo "  --skipAnalyzers            Do not run analyzers during build operations"
  echo "  --prepareMachine           Prepare machine for CI run, clean up processes after build"
  echo ""
  echo "Command line arguments starting with '/p:' are passed through to MSBuild."
}

source="${BASH_SOURCE[0]}"

# resolve $source until the file is no longer a symlink
while [[ -h "$source" ]]; do
  scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
  source="$(readlink "$source")"
  # if $source was a relative symlink, we need to resolve it relative to the path where the
  # symlink file was located
  [[ $source != /* ]] && source="$scriptroot/$source"
done
scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"

restore=false
build=false
rebuild=false
pack=false
publish=false
test_core_clr=false

configuration="Debug"
verbosity='minimal'
binary_log=false
force_bootstrap=false
ci=false
skip_analyzers=false
prepare_machine=false
properties=""

docker=false
args=""

if [[ $# = 0 ]]
then
  usage
  exit 1
fi

while [[ $# > 0 ]]; do
  opt="$(echo "$1" | awk '{print tolower($0)}')"
  case "$opt" in
    --help|-h)
      usage
      exit 0
      ;;
    --configuration|-c)
      configuration=$2
      args="$args $1"
      shift
      ;;
    --verbosity|-v)
      verbosity=$2
      args="$args $1"
      shift
      ;;
    --binarylog|-bl)
      binary_log=true
      ;;
    --bootstrap)
      force_bootstrap=true
      ;;
    --restore|-r)
      restore=true
      ;;
    --norestore)
      restore=false
      ;;
    --build|-b)
      build=true
      ;;
    --rebuild)
      rebuild=true
      ;;
    --pack)
      pack=true
      ;;
    --publish)
      publish=true
      ;;
    --testcoreclr|--test|-t)
      test_core_clr=true
      ;;
    --ci)
      ci=true
      ;;
    --skipanalyzers)
      skip_analyzers=true
      ;;
    --preparemachine)
      prepare_machine=true
      ;;
    --docker)
      docker=true
      shift
      continue
      ;;
    /p:*)
      properties="$properties $1"
      ;;
    *)
      echo "Invalid argument: $1"
      usage
      exit 1
      ;;
  esac
  args="$args $1"
  shift
done

# Import Arcade functions
. "$scriptroot/common/tools.sh"

function TestUsingNUnit() {
  testproject=""
  targetframework=""
  while [[ $# > 0 ]]; do
    opt="$(echo "$1" | awk '{print tolower($0)}')"
    case "$opt" in
      --testproject)
        testproject=$2
        shift
        ;;
      --targetframework)
        targetframework=$2
        shift
        ;;
      *)
        echo "Invalid argument: $1"
        exit 1
        ;;
    esac
    shift
  done

  if [[ "$testproject" == "" || "$targetframework" == "" ]]; then
    echo "--testproject and --targetframework must be specified"
    exit 1
  fi

  projectname=$(basename -- "$testproject")
  projectname="${projectname%.*}"
  testlogpath="$artifacts_dir/TestResults/$configuration/${projectname}_$targetframework.xml"
  args="test \"$testproject\" --no-restore --no-build -c $configuration -f $targetframework --test-adapter-path . --logger \"nunit;LogFilePath=$testlogpath\""
  "$DOTNET_INSTALL_DIR/dotnet" $args
}

function BuildSolution {
  local solution="FSharp.sln"
  echo "$solution:"

  InitializeToolset
  local toolset_build_proj=$_InitializeToolset
  
  local bl=""
  if [[ "$binary_log" = true ]]; then
    bl="/bl:\"$log_dir/Build.binlog\""
  fi
  
  local projects="$repo_root/$solution" 
  
  # https://github.com/dotnet/roslyn/issues/23736
  local enable_analyzers=!$skip_analyzers
  UNAME="$(uname)"
  if [[ "$UNAME" == "Darwin" ]]; then
    enable_analyzers=false
  fi

  # NuGet often exceeds the limit of open files on Mac and Linux
  # https://github.com/NuGet/Home/issues/2163
  if [[ "$UNAME" == "Darwin" || "$UNAME" == "Linux" ]]; then
    ulimit -n 6500
  fi

  local quiet_restore=""
  if [[ "$ci" != true ]]; then
    quiet_restore=true
  fi

  # Node reuse fails because multiple different versions of FSharp.Build.dll get loaded into MSBuild nodes
  node_reuse=false

  # build bootstrap tools
  bootstrap_config=Proto
  bootstrap_dir=$artifacts_dir/Bootstrap
  if [[ "$force_bootstrap" == true ]]; then
     rm -fr $bootstrap_dir
  fi
  if [ ! -f "$bootstrap_dir/fslex.dll" ]; then
    MSBuild "$repo_root/src/buildtools/buildtools.proj" \
      /restore \
      /v:$verbosity \
      /p:Configuration=$bootstrap_config \
      /t:Build

    mkdir -p "$bootstrap_dir"
    cp $artifacts_dir/bin/fslex/$bootstrap_config/netcoreapp2.1/* $bootstrap_dir
    cp $artifacts_dir/bin/fsyacc/$bootstrap_config/netcoreapp2.1/* $bootstrap_dir
  fi
  if [ ! -f "$bootstrap_dir/fsc.exe" ]; then
    MSBuild "$repo_root/proto.proj" \
      /restore \
      /v:$verbosity \
      /p:Configuration=$bootstrap_config \
      /t:Build

    cp $artifacts_dir/bin/fsc/$bootstrap_config/netcoreapp2.1/* $bootstrap_dir
  fi

  # do real build
  MSBuild $toolset_build_proj \
    $bl \
    /v:$verbosity \
    /p:Configuration=$configuration \
    /p:Projects="$projects" \
    /p:RepoRoot="$repo_root" \
    /p:Restore=$restore \
    /p:Build=$build \
    /p:Rebuild=$rebuild \
    /p:Pack=$pack \
    /p:Publish=$publish \
    /p:UseRoslynAnalyzers=$enable_analyzers \
    /p:ContinuousIntegrationBuild=$ci \
    /p:QuietRestore=$quiet_restore \
    /p:QuietRestoreBinaryLog="$binary_log" \
    $properties
}

InitializeDotNetCli $restore

BuildSolution

if [[ "$test_core_clr" == true ]]; then
  coreclrtestframework=netcoreapp2.1
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Compiler.UnitTests/FSharp.Compiler.UnitTests.fsproj" --targetframework $coreclrtestframework
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Build.UnitTests/FSharp.Build.UnitTests.fsproj" --targetframework $coreclrtestframework
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Core.UnitTests/FSharp.Core.UnitTests.fsproj" --targetframework $coreclrtestframework
fi

ExitWithExitCode 0

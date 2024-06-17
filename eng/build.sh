#!/usr/bin/env bash
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

# Stop script if unbound variable found (use ${var:-} if intentional)
set -u

usage()
{
  echo "Common settings:"
  echo "  --configuration <value>        Build configuration: 'Debug' or 'Release' (short: -c)"
  echo "  --verbosity <value>            Msbuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic] (short: -v)"
  echo "  --binaryLog                    Create MSBuild binary log (short: -bl)"
  echo ""
  echo "Actions:"
  echo "  --bootstrap                    Force the build of the bootstrap compiler"
  echo "  --restore                      Restore projects required to build (short: -r)"
  echo "  --norestore                    Don't restore projects required to build"
  echo "  --build                        Build all projects (short: -b)"
  echo "  --rebuild                      Rebuild all projects"
  echo "  --pack                         Build nuget packages"
  echo "  --publish                      Publish build artifacts"
  echo "  --help                         Print help and exit"
  echo ""
  echo "Test actions:"
  echo "  --testcoreclr                  Run unit tests on .NET Core (short: --test, -t)"
  echo "  --testCompilerComponentTests   Run FSharp.Compiler.ComponentTests on .NET Core"
  echo "  --testBenchmarks               Build and Run Benchmark suite"
  echo ""
  echo "Advanced settings:"
  echo "  --ci                           Building in CI"
  echo "  --docker                       Run in a docker container if applicable"
  echo "  --skipAnalyzers                Do not run analyzers during build operations"
  echo "  --skipBuild                    Do not run the build"
  echo "  --prepareMachine               Prepare machine for CI run, clean up processes after build"
  echo "  --sourceBuild                  Simulate building for source-build"
  echo "  --buildnorealsig               Build product with realsig- (default use realsig+ where necessary)"
  echo "  --tfm                          Override the default target framework"
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
test_compilercomponent_tests=false
test_benchmarks=false
configuration="Debug"
verbosity='minimal'
binary_log=false
force_bootstrap=false
ci=false
skip_analyzers=false
skip_build=false
prepare_machine=false
source_build=false
buildnorealsig=false
properties=""

docker=false
args=""

tfm="net8.0" # This needs to be changed every time it's bumped by arcade/us.

BuildCategory=""
BuildMessage=""

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
    --testcompilercomponenttests)
      test_compilercomponent_tests=true
      ;;
      --testbenchmarks)
      test_benchmarks=true
      ;;
    --ci)
      ci=true
      ;;
    --skipanalyzers)
      skip_analyzers=true
      ;;
    --skipbuild)
      skip_build=true
      ;;
    --preparemachine)
      prepare_machine=true
      ;;
    --docker)
      docker=true
      ;;
    --sourcebuild)
      source_build=true
      ;;
    --buildnorealsig)
      buildnorealsig=true
      ;;
    --tfm)
      tfm=$2
      shift
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
  BuildCategory="Test"
  BuildMessage="Error running tests"
  testproject=""
  targetframework=""
  notestfilter=0
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
      --notestfilter)
        notestfilter=1
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

  filterArgs=""
  if [[ "${RunningAsPullRequest:-}" != "true" && $notestfilter == 0 ]]; then
    filterArgs=" --filter TestCategory!=PullRequest"
  fi

  projectname=$(basename -- "$testproject")
  projectname="${projectname%.*}"
  testlogpath="$artifacts_dir/TestResults/$configuration/${projectname}_$targetframework.xml"
  args="test \"$testproject\" --no-restore --no-build -c $configuration -f $targetframework --test-adapter-path . --logger \"nunit;LogFilePath=$testlogpath\"$filterArgs --blame --results-directory $artifacts_dir/TestResults/$configuration"
  "$DOTNET_INSTALL_DIR/dotnet" $args || exit $?
}

function BuildSolution {
  BUILDING_USING_DOTNET=false
  BuildCategory="Build"
  BuildMessage="Error preparing build"

  InitializeToolset
  local toolset_build_proj=$_InitializeToolset

  local bl=""
  if [[ "$binary_log" = true ]]; then
    bl="/bl:\"$log_dir/Build.binlog\""
  fi

  local projects="$repo_root/FSharp.sln"

  echo "$projects:"

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
  # source_build=In source build proto does no work, except cause sourcebuild in wrapper to build
  bootstrap_dir=$artifacts_dir/Bootstrap
  if [[ "$force_bootstrap" == true ]]; then
    rm -fr $bootstrap_dir
  fi
  if [ ! -f "$bootstrap_dir/fslex/fslex.dll" ]; then
    local bltools=""
    if [[ "$bl" != "" ]]; then
      bltools=$bl+".proto.binlog"
    fi

    local blrestore=""
    if [[ "$source_build" != "true" ]]; then
      blrestore="/restore"
    fi

    BuildMessage="Error building tools"
    local args=" publish $repo_root/proto.proj $blrestore $bltools /p:Configuration=Proto /p:ArcadeBuildFromSource=$source_build $properties"
    echo $args
    "$DOTNET_INSTALL_DIR/dotnet" $args  #$args || exit $?
  fi

  if [[ "$skip_build" != true ]]; then
    # do real build
    BuildMessage="Error building solution"
    MSBuild $toolset_build_proj \
      $bl \
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
      /p:ArcadeBuildFromSource=$source_build \
      /p:BuildNoRealsig=$buildnorealsig \
      $properties
  fi
}

function TrapAndReportError {
  local exit_code=$?
  if [[ ! $exit_code == 0 ]]; then
    Write-PipelineTelemetryError -category $BuildCategory "$BuildMessage (exit code '$exit_code')."
    ExitWithExitCode $exit_code
  fi
}

# allow early termination to report the appropriate build failure reason
trap TrapAndReportError EXIT

InitializeDotNetCli $restore

BuildSolution

if [[ "$test_core_clr" == true ]]; then
  coreclrtestframework=$tfm
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj" --targetframework $coreclrtestframework  --notestfilter 
  TestUsingXUnit --testproject "$repo_root/tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj" --targetframework $coreclrtestframework  --notestfilter 
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Compiler.Private.Scripting.UnitTests/FSharp.Compiler.Private.Scripting.UnitTests.fsproj" --targetframework $coreclrtestframework
  TestUsingXUnit --testproject "$repo_root/tests/FSharp.Build.UnitTests/FSharp.Build.UnitTests.fsproj" --targetframework $coreclrtestframework
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Core.UnitTests/FSharp.Core.UnitTests.fsproj" --targetframework $coreclrtestframework
fi

if [[ "$test_compilercomponent_tests" == true ]]; then
  coreclrtestframework=$tfm
  TestUsingNUnit --testproject "$repo_root/tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj" --targetframework $coreclrtestframework  --notestfilter 
fi

if [[ "$test_benchmarks" == true ]]; then
  pushd "$repo_root/tests/benchmarks"
  ./SmokeTestBenchmarks.sh
  popd
fi

ExitWithExitCode 0

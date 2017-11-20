#!/bin/sh
# Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

# Helper function to print an error message and exit with a non-zero error code.
failwith () {
    printf "Error: %s\n" "$1" >&2
    exit 1
}

# Prints command text to stdout then runs (via eval) the command.
printeval () {
    printf "%s\n" "$1" >&1
    eval "$1"
}

# Prints build status to stdout.
build_status () {
    printf "%s %s %s\n" "----------------" "$1" "-----------------" >&1
}

# Text for the usage message.
usage_text="
Build and run a subset of test suites

Usage:

build.sh ^<all^|net40^|coreclr^|pcls^|vs^>
         ^<proto^|protofx^>
         ^<ci^|ci_part1^|ci_part2^|ci_part3^|microbuild^>
         ^<debug^|release^>
         ^<diag^|publicsign^>
         ^<test^|no-test^|test-net40-coreunit^|test-coreclr-coreunit^|test-compiler-unit^|test-pcl-coreunit^|test-net40-fsharp^|test-coreclr-fsharp^|test-net40-fsharpqa^>
         ^<include tag^>
         ^<init^>

No arguments default to 'default', meaning this (no testing)

    build.sh net40

Other examples:

    build.sh net40            (build compiler for .NET Framework)
    build.sh coreclr          (build compiler for .NET Core)
    build.sh vs               (build Visual Studio IDE Tools)
    build.sh all              (build everything)
    build.sh test             (build and test default targets)
    build.sh net40 test       (build and test net40)
    build.sh coreclr test     (build and test net40)
    build.sh vs test          (build and test net40)
    build.sh all test         (build and test net40)
    build.sh nobuild test include Conformance (run only tests marked with Conformance category)
    build.sh nobuild test include Expensive (run only tests marked with Expensive category)

"

# Prints usage text to stdout then exits with a non-zero exit code.
show_usage_and_exit() {
    printf "%s\n" "$usage_text"
    exit 1
}

# Check if caller specified any typical argument used to get usage/help info.
if [ "$1" = "--help" ] || [ "$1" = "-h" ] || [ "$1" = "-?" ]; then
    show_usage_and_exit
fi

# Save directory of the current script -- this is used below to fix up relative paths (if needed).
# The directory should have a trailing slash like it does on Windows, to minimize differences between scripts.
_scriptdir="$( cd -P -- "$(dirname -- "$(command -v -- "$0")")" && pwd -P )/"

# disable setup build by setting FSC_BUILD_SETUP=0
if [ -z "$FSC_BUILD_SETUP" ]; then
    export FSC_BUILD_SETUP=0
fi

# by default don't build coreclr lkg.
# However allow configuration by setting (more specifically, exporting) an environment variable: export BUILD_PROTO_WITH_CORECLR_LKG = 1
if [ -z "$BUILD_PROTO_WITH_CORECLR_LKG" ]; then
    export BUILD_PROTO_WITH_CORECLR_LKG=0
fi

export BUILD_PROTO=0
export BUILD_PHASE=1
export BUILD_NET40_FSHARP_CORE=0
export BUILD_NET40=0
export BUILD_CORECLR=0
export BUILD_PORTABLE=0
export BUILD_VS=0
export BUILD_CONFIG=release
export BUILD_CONFIG_LOWERCASE=release
export BUILD_DIAG=
export BUILD_PUBLICSIGN=0

export TEST_NET40_COMPILERUNIT_SUITE=0
export TEST_NET40_COREUNIT_SUITE=0
export TEST_NET40_FSHARP_SUITE=0
export TEST_NET40_FSHARPQA_SUITE=0
export TEST_CORECLR_COREUNIT_SUITE=0
export TEST_CORECLR_FSHARP_SUITE=0
export TEST_PORTABLE_COREUNIT_SUITE=0
export TEST_VS_IDEUNIT_SUITE=0
export INCLUDE_TEST_SPEC_NUNIT=
export INCLUDE_TEST_TAGS=

# Set up variables used to determine whether we'll automatically select which
# targets to build/run/test. NOTE: These aren't exported, they're only used by this script.
no_test=0
_autoselect=1
_autoselect_tests=0

# Parse script arguments (specifying which targets to build/run/test),
# and export the corresponding environment variables to configure the build.
for arg in "$@"
do
    case $arg in
        "net40")
            _autoselect=0
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            ;;
        "coreclr")
            _autoselect=0
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_CORECLR=1
            ;;
        "pcls")
            _autoselect=0
            export BUILD_PORTABLE=1
            ;;
        "vs")
            _autoselect=0
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_VS=1
            ;;
        "vstest")
            export TEST_VS_IDEUNIT_SUITE=1
            ;;
        "nobuild")
            export BUILD_PHASE=0
            ;;
        "none")
            _autoselect=0
            export _buildexit=1
            export _buildexitVALUE=0
            ;;
        "all")
            _autoselect=0
            export BUILD_PROTO=1
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_CORECLR=1
            export BUILD_PORTABLE=1
            export BUILD_VS=1
            export BUILD_SETUP=$FSC_BUILD_SETUP
            export CI=1
            ;;
        "microbuild")
            _autoselect=0
            export BUILD_PROTO=1
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_CORECLR=1
            export BUILD_PORTABLE=1
            export BUILD_VS=1
            export BUILD_SETUP=$FSC_BUILD_SETUP

            export TEST_NET40_COMPILERUNIT_SUITE=1
            export TEST_NET40_COREUNIT_SUITE=1
            export TEST_NET40_FSHARP_SUITE=1
            export TEST_NET40_FSHARPQA_SUITE=1
            export TEST_CORECLR_COREUNIT_SUITE=0
            export TEST_CORECLR_FSHARP_SUITE=0
            export TEST_PORTABLE_COREUNIT_SUITE=1
            export TEST_VS_IDEUNIT_SUITE=1
            export CI=1
            ;;

        # These divide 'ci' into multiple chunks which can be done in parallel
        "ci_part1")
            _autoselect=0

            # what we do
            export BUILD_PROTO=1
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_PORTABLE=1
            export BUILD_VS=1
            export BUILD_SETUP=$FSC_BUILD_SETUP
            export TEST_NET40_COMPILERUNIT_SUITE=1
            export TEST_NET40_FSHARPQA_SUITE=1
            export TEST_VS_IDEUNIT_SUITE=1
            export CI=1
            ;;
        "ci_part2")
            _autoselect=0

            # what we do
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_PROTO=1
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_PORTABLE=1

            export TEST_NET40_COREUNIT_SUITE=1
            export TEST_NET40_FSHARP_SUITE=1
            export TEST_PORTABLE_COREUNIT_SUITE=1
            export CI=1
            ;;
        "ci_part3")
            _autoselect=0

            # what we do
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_PROTO=1
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_CORECLR=1

            export TEST_CORECLR_FSHARP_SUITE=1
            export TEST_CORECLR_COREUNIT_SUITE=1
            export CI=1
            ;;

        # TEMP: The Jenkins builds currently call this script with the 'ci' target,
        #       but that doesn't exist even in build.cmd. It wasn't doing anything
        #       before anyway, so accept 'ci' but ignore it for now because the test
        #       suites don't build correctly on Linux/macOS yet anyway.
        "ci")
            # TODO: Do everything from ci_part1-3 here.
            printf "The 'ci' target is currently ignored, until the test suite builds can be fixed for Linux/macOS.\n"
            ;;

        "proto")
            export BUILD_PROTO=1            
            ;;
        "diag")
            export BUILD_DIAG=/v:detailed
            if [ -z "$APPVEYOR" ]; then
                export BUILD_LOG=fsharp_build_log.log
            fi
            ;;
        "debug")
            export BUILD_CONFIG=debug
            ;;
        "release")
            export BUILD_CONFIG=release
            ;;
        "test")
            _autoselect_tests=1
            ;;
        "no-test")
            no_test=1
            ;;
        "include")
            failwith "The 'include' option is not (yet) supported by this script."
            #export /a counter=!counter!+1
            #if '$INCLUDE_TEST_SPEC_NUNIT' == '' ( export INCLUDE_TEST_SPEC_NUNIT=cat == $ARG2 ) else (set INCLUDE_TEST_SPEC_NUNIT=cat == $ARG2 or $INCLUDE_TEST_SPEC_NUNIT )
            #if '$INCLUDE_TEST_TAGS' == '' ( export INCLUDE_TEST_TAGS=$ARG2 ) else (set INCLUDE_TEST_TAGS=$ARG2;$INCLUDE_TEST_TAGS )
            ;;
        "test-all")
            _autoselect=0
            export BUILD_PROTO=1
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_CORECLR=1
            export BUILD_PORTABLE=1
            export BUILD_VS=1
            export BUILD_SETUP=$FSC_BUILD_SETUP

            export TEST_NET40_COMPILERUNIT_SUITE=1
            export TEST_NET40_COREUNIT_SUITE=1
            export TEST_NET40_FSHARP_SUITE=1
            export TEST_NET40_FSHARPQA_SUITE=1
            export TEST_PORTABLE_COREUNIT_SUITE=1
            export TEST_CORECLR_COREUNIT_SUITE=1
            export TEST_VS_IDEUNIT_SUITE=1
            ;;
        "test-net40-fsharpqa")
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_PORTABLE=1
            export TEST_NET40_FSHARPQA_SUITE=1
            ;;
        "test-compiler-unit")
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export TEST_NET40_COMPILERUNIT_SUITE=1
            ;;
        "test-net40-coreunit")
            export BUILD_NET40_FSHARP_CORE=1
            export TEST_NET40_COREUNIT_SUITE=1
            ;;
        "test-coreclr-coreunit")
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_CORECLR=1
            export TEST_CORECLR_COREUNIT_SUITE=1
            ;;
        "test-pcl-coreunit")
            export BUILD_PORTABLE=1
            export TEST_PORTABLE_COREUNIT_SUITE=1
            ;;
        "test-net40-fsharp")
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_PORTABLE=1
            export TEST_NET40_FSHARP_SUITE=1
            ;;
        "test-coreclr-fsharp")
            export BUILD_NET40=1
            export BUILD_NET40_FSHARP_CORE=1
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            export BUILD_CORECLR=1
            export TEST_CORECLR_FSHARP_SUITE=1
            ;;
        "publicsign")
            export BUILD_PUBLICSIGN=1
            ;;
        "init")
            export BUILD_PROTO_WITH_CORECLR_LKG=1
            ;;
        *)
            errmsg=$(printf "Invalid argument: %s" "$arg")
            failwith "$errmsg"
            ;;
    esac
done

if [ $_buildexit -eq 1 ]; then
    exit $_buildexitvalue
fi

# Apply defaults, if necessary.
if [ $_autoselect -eq 1 ]; then
    export BUILD_NET40=1
    export BUILD_NET40_FSHARP_CORE=1
fi

if [ $_autoselect_tests -eq 1 ]; then
    if [ $BUILD_NET40 -eq 1 ]; then
        export TEST_NET40_COMPILERUNIT_SUITE=1
        export TEST_NET40_COREUNIT_SUITE=1
        export TEST_NET40_FSHARP_SUITE=1
        export TEST_NET40_FSHARPQA_SUITE=1
    fi

    if [ $BUILD_CORECLR -eq 1 ]; then
        export BUILD_NET40=1
        export BUILD_NET40_FSHARP_CORE=1
        export TEST_CORECLR_FSHARP_SUITE=1
        export TEST_CORECLR_COREUNIT_SUITE=1
    fi

    if [ $BUILD_PORTABLE -eq 1 ]; then
        export TEST_PORTABLE_COREUNIT_SUITE=1
    fi

    if [ $BUILD_VS -eq 1 ]; then
        export TEST_VS_IDEUNIT_SUITE=1
    fi
fi

# If the `PB_SKIPTESTS` variable is set to 'true' then no tests should be built or run, even if explicitly specified
if [ $PB_SKIPTESTS -eq "true" ]; then
    export TEST_NET40_COMPILERUNIT_SUITE=0
    export TEST_NET40_COREUNIT_SUITE=0
    export TEST_NET40_FSHARP_SUITE=0
    export TEST_NET40_FSHARPQA_SUITE=0
    export TEST_CORECLR_COREUNIT_SUITE=0
    export TEST_CORECLR_FSHARP_SUITE=0
    export TEST_PORTABLE_COREUNIT_SUITE=0
    export TEST_VS_IDEUNIT_SUITE=0
fi

#
# Report config
#

printf "Build/Tests configuration:\n"
printf "\n"
printf "BUILD_PROTO=%s\n" "$BUILD_PROTO"
printf "BUILD_PROTO_WITH_CORECLR_LKG=%s\n" "$BUILD_PROTO_WITH_CORECLR_LKG"
printf "BUILD_NET40=%s\n" "$BUILD_NET40"
printf "BUILD_NET40_FSHARP_CORE=%s\n" "$BUILD_NET40_FSHARP_CORE"
printf "BUILD_CORECLR=%s\n" "$BUILD_CORECLR"
printf "BUILD_PORTABLE=%s\n" "$BUILD_PORTABLE"
printf "BUILD_VS=%s\n" "$BUILD_VS"
printf "BUILD_SETUP=%s\n" "$BUILD_SETUP"
printf "BUILD_CONFIG=%s\n" "$BUILD_CONFIG"
printf "BUILD_PUBLICSIGN=%s\n" "$BUILD_PUBLICSIGN"
printf "\n"
printf "PB_SKIPTESTS=%s\n" "$PB_SKIPTESTS"
printf "PB_RESTORESOURCE=%s\n" "$PB_RESTORESOURCE"
printf "\n"
printf "TEST_NET40_COMPILERUNIT_SUITE=%s\n" "$TEST_NET40_COMPILERUNIT_SUITE"
printf "TEST_NET40_COREUNIT_SUITE=%s\n" "$TEST_NET40_COREUNIT_SUITE"
printf "TEST_NET40_FSHARP_SUITE=%s\n" "$TEST_NET40_FSHARP_SUITE"
printf "TEST_NET40_FSHARPQA_SUITE=%s\n" "$TEST_NET40_FSHARPQA_SUITE"
printf "TEST_CORECLR_COREUNIT_SUITE=%s\n" "$TEST_CORECLR_COREUNIT_SUITE"
printf "TEST_CORECLR_FSHARP_SUITE=%s\n" "$TEST_CORECLR_FSHARP_SUITE"
printf "TEST_PORTABLE_COREUNIT_SUITE=%s\n" "$TEST_PORTABLE_COREUNIT_SUITE"
printf "TEST_VS_IDEUNIT_SUITE=%s\n" "$TEST_VS_IDEUNIT_SUITE"
printf "INCLUDE_TEST_SPEC_NUNIT=%s\n" "$INCLUDE_TEST_SPEC_NUNIT"
printf "INCLUDE_TEST_TAGS=%s\n" "$INCLUDE_TEST_TAGS"
printf "\n"

# TODO: Print out environment variables?
printf "Environment\n"
printf "\n"
printf "(TODO)\n"
printf "\n"
printf "\n"

build_status "Done with arguments, starting preparation"

_msbuildexe="msbuild"
msbuildflags=""

# Perform any necessary setup and system configuration prior to running builds.
./before_install.sh
rc=$?;
if [ $rc -ne 0 ]; then
    printf "before_install script failed.\n"
    exit $rc
fi

build_status "Done with prepare, starting package restore"

# Use built-in Nuget executable on Mono, if available.
_nugetexe="mono .nuget/NuGet.exe"
if command -v nuget > /dev/null; then
    _nugetexe="nuget"
fi
_nugetconfig=".nuget/NuGet.Config"

# TODO: Confirm existence of 'nuget' (or $_nugetexe) before proceeding.

# Restore packages (default to restoring packages if otherwise unspecified).
if [ "${RestorePackages:-true}" = 'true' ]; then
    cd fcs
    mono .paket/paket.exe restore
    cd ..
    exit_code=$?
    if [ $exit_code -ne 0 ]; then
        exit $exit_code
    fi

    _nugetoptions="-PackagesDirectory packages -ConfigFile $_nugetconfig"
    if [ "$PB_RESTORESOURCE" != "" ]; then
        _nugetoptions="$_nugetoptions -Source $PB_RESTORESOURCE"
    fi

    eval "$_nugetexe restore packages.config $_nugetoptions"
    if [ $? -ne 0 ]; then
        failwith "Nuget restore failed"
    fi

    if [ "$BUILD_VS" = '1' ]; then
        eval "$_nugetexe restore vsintegration/packages.config $_nugetoptions"
        if [ $? -ne 0 ]; then
            failwith "Nuget restore failed"
        fi
    fi

    if [ "$BUILD_SETUP" = '1' ]; then
        eval "$_nugetexe restore setup/packages.config $_nugetoptions"
        if [ $? -ne 0 ]; then
            failwith "Nuget restore failed"
        fi
    fi
fi

# If building for CoreCLR, restore the Tools directory.
if [ "$BUILD_PROTO_WITH_CORECLR_LKG" = '1' ]; then
    # Restore the Tools directory
    ./init-tools.sh
    rc=$?;
    if [ $rc -ne 0 ]; then
        printf "init-tools script failed.\n"
        exit $rc
    fi
fi

# TODO: Check for existence of fsi (either on the system, or from the FSharp.Compiler.Tools package that was restored).

build_status "Done with package restore, starting proto"

# Decide if Proto need building
if [ ! -f "Proto/net40/bin/fsc-proto.exe" ]; then
  export BUILD_PROTO=1
fi

_dotnetexe=dotnet
_architecture=win7-x64

# Build Proto
if [ "$BUILD_PROTO" = '1' ]; then
    rm -rfd Proto

    if [ "$BUILD_PROTO_WITH_CORECLR_LKG" = '1' ]; then
        { pushd ./lkg/fsc && eval "$_dotnetexe restore" && popd; } || failwith "dotnet restore failed"
        { pushd ./lkg/fsi && eval "$_dotnetexe restore" && popd; } || failwith "dotnet restore failed"
        
        { pushd ./lkg/fsc && eval "$_dotnetexe publish project.json --no-build -o ${_scriptdir}Tools/lkg -r $_architecture" && popd; } || failwith "dotnet publish failed"
        { pushd ./lkg/fsi && eval "$_dotnetexe publish project.json --no-build -o ${_scriptdir}Tools/lkg -r $_architecture" && popd; } || failwith "dotnet publish failed"

        { printeval "$_msbuildexe $msbuildflags src/fsharp-proto-build.proj /p:Configuration=Proto"; } || failwith "compiler proto build failed"

#        { printeval "$_ngenexe install Proto/net40/bin/fsc-proto.exe /nologo"; } || failwith "NGen of proto failed"
    else
        # Build proto-compiler and libs
        { printeval "$_msbuildexe $msbuildflags src/fsharp-proto-build.proj /p:UseMonoPackaging=true /p:Configuration=Proto"; } || failwith "compiler proto build failed"
    fi
fi


build_status "Done with proto, starting build"

if [ "$BUILD_PHASE" = '1' ]; then
    cmd="$_msbuildexe $msbuildflags build-everything.proj /p:UseMonoPackaging=true /p:Configuration=$BUILD_CONFIG $BUILD_DIAG /p:BUILD_PUBLICSIGN=$BUILD_PUBLICSIGN"
    { printeval "$cmd"; } || failwith "'$cmd' failed"
fi

build_status "Main part of build finished, completing parts of build needed for Mono setup"

if [ "$BUILD_NET40" = '1' ]; then
    { printeval "./autogen.sh --prefix=/usr"; } || failwith "./autogen.sh failed"

    { printeval "make"; } || failwith "make failed"
fi

build_status "Done with build, starting update/prepare"

if [ "$BUILD_NET40" = '1' ]; then

    { printeval "sudo make install"; } || failwith "sudo make install failed"

# WINDOWS:
#    src/update.sh $BUILD_CONFIG
#    rc=$?;
#    if [ $rc -ne 0 ]; then
#        printf "src/update script failed.\n"
#        exit $rc
#    fi
fi

NUNITPATH="packages/NUnit.Console.3.0.0/tools/"
printf "set NUNITPATH=%s\n" "$NUNITPATH"
if [ ! -d "$NUNITPATH" ]; then
    failwith "Could not find $NUNITPATH"
fi

{ printeval "cp -R $NUNITPATH ${_scriptdir}tests/fsharpqa/testenv/bin/nunit"; } || failwith "unable to copy nunit binaries to fsharpqa"
{ printeval "cp -R ${_scriptdir}tests/fsharpqa/testenv/src/nunit ${_scriptdir}tests/fsharpqa/testenv/bin/nunit"; } || failwith "unable to copy nunit binaries to fsharpqa"

# TODO: Define location of ildasm/monodis and sn

if [ "$TEST_NET40_COMPILERUNIT_SUITE" = '0' ] && [ "$TEST_PORTABLE_COREUNIT_SUITE" = '0' ] && [ "$TEST_CORECLR_COREUNIT_SUITE" = '0' ] && [ "$TEST_VS_IDEUNIT_SUITE" = '0' ] && [ "$TEST_NET40_FSHARP_SUITE" = '0' ] && [ "$TEST_NET40_FSHARPQA_SUITE" = '0' ]; then
    # Successful build; not running tests so exit now.
    exit 0
fi

if [ $no_test -eq 1 ]; then
    # Successful build; not running tests so exit now.
    exit 0
fi

build_status "Done with update, starting tests"

if [ -n "$INCLUDE_TEST_SPEC_NUNIT" ]; then
    export WHERE_ARG_NUNIT="--where $INCLUDE_TEST_SPEC_NUNIT"
fi

if [ -n "$INCLUDE_TEST_TAGS" ]; then
    export TTAGS_ARG_RUNALL="-ttags:$INCLUDE_TEST_TAGS"
fi

printf "WHERE_ARG_NUNIT=%s\n" "$WHERE_ARG_NUNIT"

export NUNITPATH="tests/fsharpqa/testenv/bin/nunit/"
export NUNIT3_CONSOLE="${NUNITPATH}nunit3-console.exe"
export link_exe="${_scriptdir}packages/VisualCppTools.14.0.24519-Pre/lib/native/bin/link.exe"
if [ ! -f "$link_exe" ]; then
    failwith "failed to find '$link_exe' use nuget to restore the VisualCppTools package"
fi

#if /I not '$single_threaded' == 'true' (set PARALLEL_ARG=-procs:$NUMBER_OF_PROCESSORS) else set PARALLEL_ARG=-procs:0

export FSCBINPATH="${_scriptdir}$BUILD_CONFIG/net40/bin"
export RESULTSDIR="${_scriptdir}tests/TestResults"
if [ ! -d "$RESULTSDIR" ]; then
    mkdir "$RESULTSDIR"
fi

printf "FSCBINPATH=%s\n" "$FSCBINPATH"
printf "RESULTSDIR=%s\n" "$RESULTSDIR"
printf "link_exe=%s\n" "$link_exe"
printf "NUNIT3_CONSOLE=%s\n" "$NUNIT3_CONSOLE"
printf "NUNITPATH=%s\n" "$NUNITPATH"

# ---------------- net40-fsharp  -----------------------

if [ "$TEST_NET40_FSHARP_SUITE" = '1' ]; then
    OUTPUTARG=""
    ERRORARG=""
    OUTPUTFILE=""
    ERRORFILE=""
    XMLFILE="$RESULTSDIR/test-net40-fsharp-results.xml"
    if [ "$CI" = '1' ]; then
        OUTPUTFILE="$RESULTSDIR/test-net40-fsharp-output.log"
        OUTPUTARG="--output:\"$OUTPUTFILE\""
        ERRORFILE="$RESULTSDIR/test-net40-fsharp-errors.log"
        ERRORARG="--err:\"$ERRORFILE\""
    fi

    if ! printeval "mono $NUNIT3_CONSOLE --verbose \"$FSCBINPATH/FSharp.Tests.FSharpSuite.dll\" --framework:V4.0 --work:\"$FSCBINPATH\"  $OUTPUTARG $ERRORARG --result:\"$XMLFILE;format=nunit3\" $WHERE_ARG_NUNIT"; then
        if [ -f "$ERRORFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$ERRORFILE"
        fi
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-fsharp failed, see log above -- FAILED
        echo -----------------------------------------------------------------
        exit 1
    fi
fi

# ---------------- net40-compilerunit  -----------------------

if [ "$TEST_NET40_COMPILERUNIT_SUITE" = '1' ]; then

    OUTPUTARG=""
    ERRORARG=""
    OUTPUTFILE=""
    ERRORFILE="$RESULTSDIR/test-net40-compilerunit-errors.log"
    XMLFILE="$RESULTSDIR/test-net40-compilerunit-results.xml"
    if [ "$CI" = '1' ]; then
        OUTPUTFILE="$RESULTSDIR/test-net40-compilerunit-output.log"
	    ERRORARG="--err:\"$ERRORFILE\""
	    OUTPUTARG="--output:\"$OUTPUTFILE\""
    fi
    
    if ! printeval "mono $NUNIT3_CONSOLE --verbose --framework:V4.0 --result:\"$XMLFILE;format=nunit3\" $OUTPUTARG  $ERRORARG --work:\"$FSCBINPATH\" \"$FSCBINPATH/../../net40/bin/FSharp.Compiler.Unittests.dll\" $WHERE_ARG_NUNIT"; then
        if [ -f "$OUTPUTFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$OUTPUTFILE"
        fi
        if [ -f "$ERRORFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$ERRORFILE"
        fi
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-compilerunit failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        exit 1
    fi
fi

# ---------------- net40-coreunit  -----------------------

if [ "$TEST_NET40_COREUNIT_SUITE" = '1' ]; then

    OUTPUTARG=""
    ERRORARG=""
    OUTPUTFILE=""
    ERRORFILE=""
    XMLFILE="$RESULTSDIR/test-net40-coreunit-results.xml"
    if [ "$CI" = '1' ]; then
        ERRORFILE="$RESULTSDIR/test-net40-coreunit-errors.log"
        OUTPUTFILE="$RESULTSDIR/test-net40-coreunit-output.log"
	    ERRORARG="--err:\"$ERRORFILE\""
	    OUTPUTARG="--output:\"$OUTPUTFILE\""
    fi

    if ! printeval "mono $NUNIT3_CONSOLE --verbose --framework:V4.0 --result:\"$XMLFILE;format=nunit3\" $OUTPUTARG $ERRORARG --work:\"$FSCBINPATH\" \"$FSCBINPATH/FSharp.Core.Unittests.dll\" $WHERE_ARG_NUNIT"; then
        if [ -f "$OUTPUTFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$OUTPUTFILE"
        fi
        if [ -f "$ERRORFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$ERRORFILE"
        fi
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-coreunit failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        exit 1
    fi
fi

#  ---------------- portable-coreunit  -----------------------

if [ "$TEST_PORTABLE_COREUNIT_SUITE" = '1' ]; then

    OUTPUTARG=""
    ERRORARG=""
    OUTPUTFILE=""
    ERRORFILE=""
    XMLFILE="$RESULTSDIR/test-portable-coreunit-results.xml"

    if [ "$CI" = '1' ]; then
    	OUTPUTFILE="$RESULTSDIR/test-portable-coreunit-output.log"
	    ERRORFILE="$RESULTSDIR/test-portable-coreunit-errors.log"
        ERRORARG="--err:\"$ERRORFILE\""
        OUTPUTARG="--output:\"$OUTPUTFILE\""
	fi

    if ! printeval "mono $NUNIT3_CONSOLE /framework:V4.0 /result=\"$XMLFILE;format=nunit3\" $OUTPUTARG $ERRORARG /work=\"$FSCBINPATH\" \"$FSCBINPATH/../../portable7/bin/FSharp.Core.Unittests.dll\" \"$FSCBINPATH/../../portable47/bin/FSharp.Core.Unittests.dll\" \"$FSCBINPATH/../../portable78/bin/FSharp.Core.Unittests.dll\" \"$FSCBINPATH/../../portable259/bin/FSharp.Core.Unittests.dll\" $WHERE_ARG_NUNIT"; then
        if [ -f "$OUTPUTFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$OUTPUTFILE"
        fi
        if [ -f "$ERRORFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$ERRORFILE"
        fi
        echo -----------------------------------------------------------------
        echo Error: Running tests portable-coreunit failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        exit 1
    fi
fi

#  ---------------- coreclr-coreunit  -----------------------

if [ "$TEST_CORECLR_COREUNIT_SUITE" = '1' ]; then

    XMLFILE="$RESULTSDIR/test-coreclr-coreunit-results.xml"
    OUTPUTFILE="$RESULTSDIR/test-coreclr-coreunit-output.log"
    ERRORFILE="$RESULTSDIR/test-coreclr-coreunit-errors.log"

    if ! printeval "$_dotnetexe \"${_scriptdir}tests/testbin/$BUILD_CONFIG/coreclr/FSharp.Core.Unittests/FSharp.Core.Unittests.dll\" $WHERE_ARG_NUNIT"; then
        echo -----------------------------------------------------------------
        echo Error: Running tests coreclr-coreunit failed, see logs above-- FAILED
        echo -----------------------------------------------------------------
        exit 1
    fi
fi

# ---------------- coreclr-fsharp  -----------------------

if [ "$TEST_CORECLR_FSHARP_SUITE" = '1' ]; then

    export single_threaded=true
    export permutations=FSC_CORECLR

    OUTPUTARG=""
    ERRORARG=""
    OUTPUTFILE=""
    ERRORFILE=""
    XMLFILE="$RESULTSDIR/test-coreclr-fsharp-results.xml"
    if ! printeval "$_dotnetexe \"${_scriptdir}tests/testbin/$BUILD_CONFIG/coreclr/FSharp.Tests.FSharpSuite.DrivingCoreCLR/FSharp.Tests.FSharpSuite.DrivingCoreCLR.dll\" $WHERE_ARG_NUNIT"; then
        echo -----------------------------------------------------------------
        echo Error: Running tests coreclr-fsharp failed, see logs above-- FAILED
        echo -----------------------------------------------------------------
        exit 1
    fi
fi

# ---------------- net40-fsharpqa  -----------------------

OSARCH="${PROCESSOR_ARCHITECTURE:-x64}"

# Set this to 1 in order to use an external compiler host process
#    This only has an effect when running the FSHARPQA tests, but can
#    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
HOSTED_COMPILER=1

if [ "$TEST_NET40_FSHARPQA_SUITE" = '1' ]; then

	export FSC="$FSCBINPATH/fsc.exe"
	export FSCOREDLLPATH="$FSCBINPATH/FSharp.Core.dll"
	export PATH="$FSCBINPATH;$PATH"

    if ! command -v perl > /dev/null; then
		failwith "perl is not in the PATH, it is required for the net40-fsharpqa test suite"
	fi

	OUTPUTFILE=test-net40-fsharpqa-results.log
	ERRORFILE=test-net40-fsharpqa-errors.log
	FAILENV=test-net40-fsharpqa-errors

	{ pushd "${_scriptdir}tests/fsharpqa/source" && \
      printeval "perl tests/fsharpqa/testenv/bin/runall.pl -resultsroot $RESULTSDIR -results $OUTPUTFILE -log $ERRORFILE -fail $FAILENV -cleanup:no $TTAGS_ARG_RUNALL $PARALLEL_ARG" && \
      popd; } || {
        if [ -f "$RESULTSDIR/$OUTPUTFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$RESULTSDIR/$OUTPUTFILE"
        fi
        if [ -f "$RESULTSDIR/$ERRORFILE" ]; then
            echo -----------------------------------------------------------------
            cat "$RESULTSDIR/$ERRORFILE"
        fi
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-fsharpqa failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        exit 1
      }
fi

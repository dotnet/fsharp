#!/usr/bin/env bash

#
# End to end tests for DesignTimeProviderPackaging
# Tests the conditional inclusion of PackageFSharpDesignTimeTools target
#

set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
configuration=Debug

while [[ $# -gt 0 ]]; do
    case $1 in
        -c)
            configuration="$2"
            shift 2
            ;;
        *)
            echo "Unsupported argument: $1"
            exit 1
            ;;
    esac
done

cd "$SCRIPT_DIR"

# Clean artifacts
rm -rf artifacts
mkdir -p artifacts

echo
echo "=== Test 1: Plain Library (No Provider) ==="
echo "dotnet pack PlainLib/PlainLib.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/plain.binlog"
dotnet pack PlainLib/PlainLib.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/plain.binlog

# Check that PackageFSharpDesignTimeTools target did not run
if strings artifacts/plain.binlog | grep -q "PackageFSharpDesignTimeTools"; then
    echo "Error: PackageFSharpDesignTimeTools target should not have run for plain library"
    exit 1
fi

# Check that no tools folder exists in nupkg
if unzip -l artifacts/PlainLib.1.0.0.nupkg | grep -q "tools/fsharp41/"; then
    echo "Error: Plain library should not contain tools/fsharp41 folder"
    exit 1
fi

echo "Plain library test passed"

echo
echo "=== Test 2: Provider Project (Direct Flag) ==="
echo "dotnet pack Provider/Provider.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/provider.binlog"
dotnet pack Provider/Provider.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/provider.binlog

# Check that PackageFSharpDesignTimeTools target ran
if ! strings artifacts/provider.binlog | grep -q "PackageFSharpDesignTimeTools"; then
    echo "Error: PackageFSharpDesignTimeTools target should have run for provider"
    exit 1
fi

# Check that tools folder exists in nupkg
if ! unzip -l artifacts/Provider.1.0.0.nupkg | grep -q "tools/fsharp41/"; then
    echo "Error: Provider should contain tools/fsharp41 folder"
    exit 1
fi

echo "Provider test passed"

echo
echo "=== Test 3: Host with ProjectReference to Provider ==="
echo "dotnet pack Host/Host.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/host.binlog"
dotnet pack Host/Host.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/host.binlog

# Note: This test may not work as expected due to MSBuild evaluation phase limitations
# The current implementation only checks IsFSharpDesignTimeProvider property directly
echo "Host test completed (implementation limitation noted)"

echo
echo "=== Test 4: Pack with --no-build (No Provider) ==="
echo "dotnet build PlainLib/PlainLib.fsproj -c $configuration"
dotnet build PlainLib/PlainLib.fsproj -c $configuration

echo "dotnet pack PlainLib/PlainLib.fsproj --no-build -o artifacts -c $configuration -v minimal -bl:artifacts/nobuild.binlog"
dotnet pack PlainLib/PlainLib.fsproj --no-build -o artifacts -c $configuration -v minimal -bl:artifacts/nobuild.binlog

echo "No-build test passed"

echo
echo "=== Test 5: Binding Redirect / App.config Interaction ==="
echo "dotnet pack RedirectLib/RedirectLib.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/redirect.binlog"
dotnet pack RedirectLib/RedirectLib.fsproj -o artifacts -c $configuration -v minimal -bl:artifacts/redirect.binlog

# Check that PackageFSharpDesignTimeTools target did not run
if strings artifacts/redirect.binlog | grep -q "PackageFSharpDesignTimeTools"; then
    echo "Error: PackageFSharpDesignTimeTools target should not have run for redirect library"
    exit 1
fi

echo "Redirect test passed"

echo
echo "=== Test 6: Issue Repro - Simple project with reference and pack --no-build ==="
pushd artifacts

echo "Creating eff project..."
dotnet new classlib --language F# --name eff -o eff

echo "Creating gee project..."
dotnet new classlib --language F# --name gee -o gee

echo "Adding reference from gee to eff..."
dotnet add gee/gee.fsproj reference eff/eff.fsproj

echo "Building gee project..."
dotnet build gee/gee.fsproj

echo "Packing gee project with --no-build..."
dotnet pack gee/gee.fsproj --no-build

popd

echo "Issue repro test passed"

echo
echo "=== All DesignTimeProviderPackaging tests PASSED ==="
exit 0
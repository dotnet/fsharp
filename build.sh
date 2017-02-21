#!/bin/sh

# At the moment all we build is the Mono version of the F# compiler
export BUILD_NET40=1

# Perform any necessary setup prior to running builds
# (e.g., restoring NuGet packages).
./before_install.sh
rc=$?;
if [ $rc -ne 0 ]; then
    echo "before_install script failed."
    exit $rc
fi

# This is a very, very limited build script for Mono which bootstraps the compiler
echo "xbuild src/fsharp-proto-build.proj  /p:UseMonoPackaging=true"
xbuild src/fsharp-proto-build.proj  /p:UseMonoPackaging=true
echo "xbuild build-everything.proj /p:Configuration=release /p:UseMonoPackaging=true"
xbuild build-everything.proj /p:Configuration=release /p:UseMonoPackaging=true

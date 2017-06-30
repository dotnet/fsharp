#!/bin/sh

# Perform any necessary setup prior to running builds
# (e.g., restoring NuGet packages).
echo "prepare-mono.sh..."

./mono/prepare-mono.sh

rc=$?;
if [ $rc -ne 0 ]; then
    echo "mono/prepare-mono script failed."
    exit $rc
fi
echo "done mono/prepare-mono.sh, building..."

chmod +x mono/travis-autogen.sh

# Generate the makefiles 
# Bootstrap the compiler
# Install the compiler
./mono/travis-autogen.sh && \
make && \
sudo make install


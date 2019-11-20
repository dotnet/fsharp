#!/bin/bash

# bail out as soon as any single command errors
set -e

start_pwd=$PWD

# dotnet tools look in certain paths by default that Just Work when we're in the fcs dir,
# so let's force that here:
cd $(dirname ${BASH_SOURCE[0]})

dotnet tool restore
dotnet paket restore
dotnet fake build -t $@

# but we'll be nice and go back to the start dir at the end
cd $start_pwd
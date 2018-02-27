#!/bin/bash

# note: expects to run from top directory
./mono/latest-mono-stable.sh
make Configuration=$@
#sudo make install Configuration=$@
#./mono/test-mono.sh
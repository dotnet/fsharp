#!/bin/bash

# suitable for Ubuntu 16.04 - get latest Mono stable
mono --version
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/ubuntu stable-xenial main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt-get update
ps -A | grep apt
#sudo rm /var/lib/dpkg/lock
#sudo dpkg --configure -a
sudo apt-get -my install mono-devel
mono --version

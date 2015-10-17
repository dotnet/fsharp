# http://dotnet.readthedocs.org/en/latest/getting-started/installing-core-windows.html
&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}
dnvm version
dnvm install -r coreclr -arch x64 latest -u
dnx --version
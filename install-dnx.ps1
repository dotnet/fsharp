# http://dotnet.readthedocs.org/en/latest/getting-started/installing-core-windows.html
&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}
$dnvm = ls ~/.dnx/bin/dnvm.cmd
echo "dnvm is $dnvm"
$env:path = "$dnvm.Parent.FullName;$env:path"
echo "path is $env:path"
dnvm version
dnvm install -r coreclr -arch x64 1.0.0-beta8
dnx --version
rem if you want to try it out without running the whole suite
csc -t:library -out:liba.dll liba-and-b.cs
csc -t:library -out:libb.dll liba-and-b.cs
fsc --target:library -r:liba.dll --out:libc.dll libc.fs
fsc --target:library -r:libb.dll --out:libd.dll libd.fs
fsc -r:liba.dll -r:libb.dll -r:libc.dll -r:libd.dll app.fs

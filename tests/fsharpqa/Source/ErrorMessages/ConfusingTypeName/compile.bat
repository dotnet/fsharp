rem if you want to try it out without running the whole suite
csc liba-and-b.cs -t:library -out:liba.dll
csc liba-and-b.cs -t:library -out:libb.dll
fsc libc.fs --target:library -r:liba.dll --out:libc.dll
fsc libd.fs --target:library -r:libb.dll --out:libd.dll
fsc app.fs -r:liba.dll -r:libb.dll -r:libc.dll -r:libd.dll

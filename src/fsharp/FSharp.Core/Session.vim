let SessionLoad = 1
let s:so_save = &so | let s:siso_save = &siso | set so=0 siso=0
let v:this_session=expand("<sfile>:p")
silent only
cd ~\git\visualfsharp\src\fsharp\FSharp.Core
if expand('%') == '' && !&modified && line('$') <= 1 && getline(1) == ''
  let s:wipebuf = bufnr('%')
endif
set shortmess=aoO
badd +1903 printf.fs
badd +1 .vim\coc-settings.json
badd +79 ~\git\visualfsharp\Session.vim
argglobal
silent! argdel *
edit printf.fs
set splitbelow splitright
wincmd _ | wincmd |
split
1wincmd k
wincmd w
set nosplitbelow
set nosplitright
wincmd t
set winminheight=0
set winheight=1
set winminwidth=0
set winwidth=1
exe '1resize ' . ((&lines * 29 + 30) / 61)
exe '2resize ' . ((&lines * 28 + 30) / 61)
argglobal
setlocal fdm=indent
setlocal fde=0
setlocal fmr={{{,}}}
setlocal fdi=#
setlocal fdl=10
setlocal fml=1
setlocal fdn=20
setlocal fen
29
normal! zo
279
normal! zo
1221
normal! zo
1221
normal! zc
1318
normal! zo
1318
normal! zc
1434
normal! zo
1434
normal! zc
1507
normal! zo
1507
normal! zc
1557
normal! zo
1557
normal! zc
1585
normal! zc
1595
normal! zo
1595
normal! zc
1626
normal! zo
1626
normal! zc
1642
normal! zo
1703
normal! zo
1712
normal! zo
1752
normal! zo
1752
normal! zc
1791
normal! zo
1791
normal! zc
1816
normal! zo
1816
normal! zc
1834
normal! zo
1834
normal! zc
1878
normal! zo
1882
normal! zo
1896
normal! zo
1925
normal! zo
1955
normal! zo
1975
normal! zo
let s:l = 1894 - ((16 * winheight(0) + 14) / 29)
if s:l < 1 | let s:l = 1 | endif
exe s:l
normal! zt
1894
normal! 0
lcd ~\git\visualfsharp\src\fsharp\FSharp.Core
wincmd w
argglobal
if bufexists("~\git\visualfsharp\src\fsharp\FSharp.Core\printf.fs") | buffer ~\git\visualfsharp\src\fsharp\FSharp.Core\printf.fs | else | edit ~\git\visualfsharp\src\fsharp\FSharp.Core\printf.fs | endif
setlocal fdm=indent
setlocal fde=0
setlocal fmr={{{,}}}
setlocal fdi=#
setlocal fdl=10
setlocal fml=1
setlocal fdn=20
setlocal fen
29
normal! zo
279
normal! zo
1221
normal! zo
1221
normal! zc
1318
normal! zo
1318
normal! zc
1434
normal! zo
1434
normal! zc
1507
normal! zo
1507
normal! zc
1557
normal! zo
1557
normal! zc
1585
normal! zc
1595
normal! zo
1595
normal! zc
1626
normal! zo
1626
normal! zc
1642
normal! zo
1703
normal! zo
1712
normal! zo
1712
normal! zc
1752
normal! zo
1752
normal! zc
1791
normal! zo
1791
normal! zc
1816
normal! zo
1834
normal! zo
1834
normal! zc
1878
normal! zo
1882
normal! zo
1896
normal! zo
1925
normal! zo
1955
normal! zo
1975
normal! zo
let s:l = 1817 - ((122 * winheight(0) + 14) / 28)
if s:l < 1 | let s:l = 1 | endif
exe s:l
normal! zt
1817
normal! 021|
lcd ~\git\visualfsharp\src\fsharp\FSharp.Core
wincmd w
2wincmd w
exe '1resize ' . ((&lines * 29 + 30) / 61)
exe '2resize ' . ((&lines * 28 + 30) / 61)
tabnext 1
if exists('s:wipebuf') && getbufvar(s:wipebuf, '&buftype') isnot# 'terminal'
  silent exe 'bwipe ' . s:wipebuf
endif
unlet! s:wipebuf
set winheight=1 winwidth=20 winminheight=1 winminwidth=1 shortmess=filnxtToOFI
let s:sx = expand("<sfile>:p:r")."x.vim"
if file_readable(s:sx)
  exe "source " . fnameescape(s:sx)
endif
let &so = s:so_save | let &siso = s:siso_save
doautoall SessionLoadPost
unlet SessionLoad
" vim: set ft=vim :

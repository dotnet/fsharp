let SessionLoad = 1
let s:so_save = &so | let s:siso_save = &siso | set so=0 siso=0
let v:this_session=expand("<sfile>:p")
silent only
cd ~\git\visualfsharp
if expand('%') == '' && !&modified && line('$') <= 1 && getline(1) == ''
  let s:wipebuf = bufnr('%')
endif
set shortmess=aoO
badd +1579 src\fsharp\FSharp.Core\printf.fs
argglobal
silent! argdel *
edit src\fsharp\FSharp.Core\printf.fs
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
exe '1resize ' . ((&lines * 23 + 25) / 50)
exe '2resize ' . ((&lines * 23 + 25) / 50)
argglobal
setlocal fdm=indent
setlocal fde=0
setlocal fmr={{{,}}}
setlocal fdi=#
setlocal fdl=10
setlocal fml=1
setlocal fdn=20
setlocal fen
7
normal! zo
7
normal! zo
29
normal! zo
228
normal! zo
278
normal! zo
327
normal! zo
332
normal! zo
333
normal! zo
342
normal! zo
347
normal! zo
348
normal! zo
358
normal! zo
909
normal! zo
909
normal! zc
996
normal! zo
996
normal! zc
1006
normal! zo
1006
normal! zc
1122
normal! zo
1122
normal! zc
1147
normal! zo
1147
normal! zc
1162
normal! zc
1195
normal! zo
1195
normal! zc
1245
normal! zo
1245
normal! zc
1273
normal! zc
1283
normal! zo
1283
normal! zc
1314
normal! zo
1314
normal! zc
1391
normal! zo
1400
normal! zo
1418
normal! zo
1440
normal! zo
1456
normal! zo
1479
normal! zo
1504
normal! zo
1522
normal! zo
1541
normal! zo
1565
normal! zo
1568
normal! zo
1597
normal! zo
1607
normal! zo
1627
normal! zo
1754
normal! zo
1796
normal! zo
let s:l = 380 - ((22 * winheight(0) + 11) / 23)
if s:l < 1 | let s:l = 1 | endif
exe s:l
normal! zt
380
normal! 0
wincmd w
argglobal
if bufexists("src\fsharp\FSharp.Core\printf.fs") | buffer src\fsharp\FSharp.Core\printf.fs | else | edit src\fsharp\FSharp.Core\printf.fs | endif
setlocal fdm=indent
setlocal fde=0
setlocal fmr={{{,}}}
setlocal fdi=#
setlocal fdl=10
setlocal fml=1
setlocal fdn=20
setlocal fen
7
normal! zo
7
normal! zo
29
normal! zo
228
normal! zo
278
normal! zo
327
normal! zo
332
normal! zo
333
normal! zo
342
normal! zo
347
normal! zo
348
normal! zo
358
normal! zo
909
normal! zo
909
normal! zc
996
normal! zo
996
normal! zc
1006
normal! zo
1006
normal! zc
1122
normal! zo
1122
normal! zc
1147
normal! zo
1147
normal! zc
1162
normal! zc
1195
normal! zo
1195
normal! zc
1245
normal! zo
1245
normal! zc
1273
normal! zc
1283
normal! zo
1283
normal! zc
1314
normal! zo
1314
normal! zc
1391
normal! zo
1400
normal! zo
1418
normal! zo
1440
normal! zo
1456
normal! zo
1479
normal! zo
1504
normal! zo
1522
normal! zo
1541
normal! zo
1544
normal! zo
1544
normal! zc
1541
normal! zc
1565
normal! zo
1568
normal! zo
1565
normal! zc
1597
normal! zo
1607
normal! zo
1627
normal! zo
1754
normal! zo
1796
normal! zo
let s:l = 1643 - ((6 * winheight(0) + 11) / 23)
if s:l < 1 | let s:l = 1 | endif
exe s:l
normal! zt
1643
normal! 0
wincmd w
2wincmd w
exe '1resize ' . ((&lines * 23 + 25) / 50)
exe '2resize ' . ((&lines * 23 + 25) / 50)
tabnext 1
if exists('s:wipebuf') && getbufvar(s:wipebuf, '&buftype') isnot# 'terminal'
  silent exe 'bwipe ' . s:wipebuf
endif
unlet! s:wipebuf
set winheight=1 winwidth=20 winminheight=1 winminwidth=1 shortmess=filnxtToOF
let s:sx = expand("<sfile>:p:r")."x.vim"
if file_readable(s:sx)
  exe "source " . fnameescape(s:sx)
endif
let &so = s:so_save | let &siso = s:siso_save
doautoall SessionLoadPost
unlet SessionLoad
" vim: set ft=vim :

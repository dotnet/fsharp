let SessionLoad = 1
let s:so_save = &so | let s:siso_save = &siso | set so=0 siso=0
let v:this_session=expand("<sfile>:p")
silent only
cd ~\git\visualfsharp\src\fsharp\FSharp.Core
if expand('%') == '' && !&modified && line('$') <= 1 && getline(1) == ''
  let s:wipebuf = bufnr('%')
endif
set shortmess=aoO
badd +682 printf.fs
badd +1 .vim\coc-settings.json
badd +79 ~\git\visualfsharp\Session.vim
argglobal
silent! argdel *
edit printf.fs
set splitbelow splitright
set nosplitbelow
set nosplitright
wincmd t
set winminheight=0
set winheight=1
set winminwidth=0
set winwidth=1
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
let s:l = 1686 - ((94 * winheight(0) + 29) / 58)
if s:l < 1 | let s:l = 1 | endif
exe s:l
normal! zt
1686
normal! 016|
lcd ~\git\visualfsharp\src\fsharp\FSharp.Core
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

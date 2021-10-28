
test-langversion-preview.fsx(2,3,2,5): typecheck info FS3370: The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.

test-langversion-preview.fsx(3,10,3,11): typecheck info FS3370: The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.

test-langversion-preview.fsx(4,1,4,5): typecheck info FS3370: The use of 'incr' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'incr cell' to 'cell.Value <- cell.Value + 1'.

test-langversion-preview.fsx(5,1,5,5): typecheck info FS3370: The use of 'decr' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'decr cell' to 'cell.Value <- cell.Value - 1'.

test-langversion-preview.fsx(9,14,9,15): typecheck error FS0438: Duplicate method. The method 'M' has the same name and signature as another method in type 'X'.

test-langversion-preview.fsx(8,14,8,15): typecheck error FS0438: Duplicate method. The method 'M' has the same name and signature as another method in type 'X'.

#if PIPED_FROM_TEST_DIR
#load "aaa/RelativeHashRResolution03_2.fsx"
#else
#load "../aaa/RelativeHashRResolution03_2.fsx"
#endif

printfn "%O" (Lib.X())
printfn "%O" RelativeHashRResolution03_2.Foo.Y

#q ;;
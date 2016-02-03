#if PIPED_FROM_TEST_DIR
#load "aaa/bbb/RelativeHashRResolution01_2.fsx"
#else
#load "../aaa/bbb/RelativeHashRResolution01_2.fsx"
#endif

printfn "%O" (Lib.X())
printfn "%O" RelativeHashRResolution01_2.Foo.Y

#q ;;
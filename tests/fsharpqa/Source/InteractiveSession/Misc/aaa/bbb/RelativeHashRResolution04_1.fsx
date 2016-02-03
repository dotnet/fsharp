#if PIPED_FROM_TEST_DIR
#load "aaa/bbb/RelativeHashRResolution04_2.fsx"
#else
#load "RelativeHashRResolution04_2.fsx"
#endif

printfn "%O" (Lib.X())
printfn "%O" RelativeHashRResolution04_2.Foo.Y

#q ;;
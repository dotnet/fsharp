#if PIPED_FROM_TEST_DIR
#load "./aaa/RelativeHashRResolution05_2.fsx"
#else
#load "../RelativeHashRResolution05_2.fsx"
#endif

printfn "%O" (Lib.X())
printfn "%O" RelativeHashRResolution05_2.Foo.Y

#q ;;
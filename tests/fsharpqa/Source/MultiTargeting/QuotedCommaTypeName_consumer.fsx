#if INTERACTIVE
#r "author.dll"
#else
module Test
#endif

printfn "%A" Foo.X

#if INTERACTIVE
#q ;;
#endif
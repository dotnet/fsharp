ImplFile
  (ParsedImplFileInput
     ("/root/TupleInLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile TupleInLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([TupleInLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id
                       (x, None, false, false, false,
                        /root/TupleInLambdaHasArrowRange.fs (1,5--1,6));
                     Id
                       (_arg1, None, true, false, false,
                        /root/TupleInLambdaHasArrowRange.fs (1,8--1,9))],
                    /root/TupleInLambdaHasArrowRange.fs (1,4--1,10)),
                 App
                   (NonAtomic, false,
                    App
                      (NonAtomic, true,
                       LongIdent
                         (false,
                          SynLongIdent
                            ([op_Multiply], [], [Some (OriginalNotation "*")]),
                          None, /root/TupleInLambdaHasArrowRange.fs (1,16--1,17)),
                       Ident x, /root/TupleInLambdaHasArrowRange.fs (1,14--1,17)),
                    Const
                      (Int32 3, /root/TupleInLambdaHasArrowRange.fs (1,18--1,19)),
                    /root/TupleInLambdaHasArrowRange.fs (1,14--1,19)),
                 Some
                   ([Paren
                       (Tuple
                          (false,
                           [Named
                              (SynIdent (x, None), false, None,
                               /root/TupleInLambdaHasArrowRange.fs (1,5--1,6));
                            Wild /root/TupleInLambdaHasArrowRange.fs (1,8--1,9)],
                           /root/TupleInLambdaHasArrowRange.fs (1,5--1,9)),
                        /root/TupleInLambdaHasArrowRange.fs (1,4--1,10))],
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Multiply], [], [Some (OriginalNotation "*")]),
                             None,
                             /root/TupleInLambdaHasArrowRange.fs (1,16--1,17)),
                          Ident x,
                          /root/TupleInLambdaHasArrowRange.fs (1,14--1,17)),
                       Const
                         (Int32 3,
                          /root/TupleInLambdaHasArrowRange.fs (1,18--1,19)),
                       /root/TupleInLambdaHasArrowRange.fs (1,14--1,19))),
                 /root/TupleInLambdaHasArrowRange.fs (1,0--1,19),
                 { ArrowRange =
                    Some /root/TupleInLambdaHasArrowRange.fs (1,11--1,13) }),
              /root/TupleInLambdaHasArrowRange.fs (1,0--1,19))], PreXmlDocEmpty,
          [], None, /root/TupleInLambdaHasArrowRange.fs (1,0--1,19),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
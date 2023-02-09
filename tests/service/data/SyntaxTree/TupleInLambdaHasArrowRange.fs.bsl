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
                        /root/TupleInLambdaHasArrowRange.fs (2,5--2,6));
                     Id
                       (_arg1, None, true, false, false,
                        /root/TupleInLambdaHasArrowRange.fs (2,8--2,9))],
                    /root/TupleInLambdaHasArrowRange.fs (2,4--2,10)),
                 App
                   (NonAtomic, false,
                    App
                      (NonAtomic, true,
                       LongIdent
                         (false,
                          SynLongIdent
                            ([op_Multiply], [], [Some (OriginalNotation "*")]),
                          None, /root/TupleInLambdaHasArrowRange.fs (2,16--2,17)),
                       Ident x, /root/TupleInLambdaHasArrowRange.fs (2,14--2,17)),
                    Const
                      (Int32 3, /root/TupleInLambdaHasArrowRange.fs (2,18--2,19)),
                    /root/TupleInLambdaHasArrowRange.fs (2,14--2,19)),
                 Some
                   ([Paren
                       (Tuple
                          (false,
                           [Named
                              (SynIdent (x, None), false, None,
                               /root/TupleInLambdaHasArrowRange.fs (2,5--2,6));
                            Wild /root/TupleInLambdaHasArrowRange.fs (2,8--2,9)],
                           /root/TupleInLambdaHasArrowRange.fs (2,5--2,9)),
                        /root/TupleInLambdaHasArrowRange.fs (2,4--2,10))],
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Multiply], [], [Some (OriginalNotation "*")]),
                             None,
                             /root/TupleInLambdaHasArrowRange.fs (2,16--2,17)),
                          Ident x,
                          /root/TupleInLambdaHasArrowRange.fs (2,14--2,17)),
                       Const
                         (Int32 3,
                          /root/TupleInLambdaHasArrowRange.fs (2,18--2,19)),
                       /root/TupleInLambdaHasArrowRange.fs (2,14--2,19))),
                 /root/TupleInLambdaHasArrowRange.fs (2,0--2,19),
                 { ArrowRange =
                    Some /root/TupleInLambdaHasArrowRange.fs (2,11--2,13) }),
              /root/TupleInLambdaHasArrowRange.fs (2,0--2,19))], PreXmlDocEmpty,
          [], None, /root/TupleInLambdaHasArrowRange.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
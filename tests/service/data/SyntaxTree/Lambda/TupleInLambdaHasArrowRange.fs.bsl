ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/TupleInLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile TupleInLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([TupleInLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (x, None, false, false, false, (2,5--2,6));
                     Id (_arg1, None, true, false, false, (2,8--2,9))],
                    (2,4--2,10)),
                 App
                   (NonAtomic, false,
                    App
                      (NonAtomic, true,
                       LongIdent
                         (false,
                          SynLongIdent
                            ([op_Multiply], [], [Some (OriginalNotation "*")]),
                          None, (2,16--2,17)), Ident x, (2,14--2,17)),
                    Const (Int32 3, (2,18--2,19)), (2,14--2,19)),
                 Some
                   ([Paren
                       (Tuple
                          (false,
                           [Named (SynIdent (x, None), false, None, (2,5--2,6));
                            Wild (2,8--2,9)], (2,5--2,9)), (2,4--2,10))],
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Multiply], [], [Some (OriginalNotation "*")]),
                             None, (2,16--2,17)), Ident x, (2,14--2,17)),
                       Const (Int32 3, (2,18--2,19)), (2,14--2,19))),
                 (2,0--2,19), { ArrowRange = Some (2,11--2,13) }), (2,0--2,19))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

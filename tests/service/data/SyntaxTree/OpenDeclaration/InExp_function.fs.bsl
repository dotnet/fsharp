ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_function.fs", false,
      QualifiedNameOfFile InExp_function, [],
      [SynModuleOrNamespace
         ([InExp_function], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (fun2, None), false, None, (1,4--1,8)), None,
                  MatchLambda
                    (false, (1,11--1,19),
                     [SynMatchClause
                        (Named (SynIdent (x, None), false, None, (1,20--1,21)),
                         None,
                         Open
                           (Type
                              (LongIdent
                                 (SynLongIdent
                                    ([System; Int32], [(1,41--1,42)],
                                     [None; None])), (1,35--1,47)), (1,25--1,47),
                            (1,25--1,61),
                            App
                              (NonAtomic, false,
                               App
                                 (NonAtomic, true,
                                  LongIdent
                                    (false,
                                     SynLongIdent
                                       ([op_Addition], [],
                                        [Some (OriginalNotation "+")]), None,
                                     (1,51--1,52)), Ident x, (1,49--1,52)),
                               Ident MinValue, (1,49--1,61))), (1,20--1,61), Yes,
                         { ArrowRange = Some (1,22--1,24)
                           BarRange = None })], NoneAtInvisible, (1,11--1,61)),
                  (1,4--1,8), NoneAtLet, { LeadingKeyword = Let (1,0--1,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (1,9--1,10) })],
              (1,0--1,61))], PreXmlDocEmpty, [], None, (1,0--1,61),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

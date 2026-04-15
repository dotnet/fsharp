ImplFile
  (ParsedImplFileInput
     ("/root/Expression/LetIn 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Const (Int32 42, (3,8--3,10)), (3,4--3,5), Yes (3,0--3,10),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,10),
              { InKeyword = None });
           Expr
             (Do
                (Sequential
                   (SuppressNeither, true,
                    LetOrUse
                      { IsRecursive = false
                        Bindings =
                         [SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named (SynIdent (x, None), false, None, (5,8--5,9)),
                             None, Const (Int32 1, (5,12--5,13)), (5,8--5,9),
                             Yes (5,4--5,13),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,10--5,11) })]
                        Body =
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  (5,19--5,20)), Ident x, (5,17--5,20)),
                            Const (Int32 1, (5,21--5,22)), (5,17--5,22))
                        Range = (5,4--5,22)
                        Trivia = { InKeyword = Some (5,14--5,16) }
                        IsFromSource = true }, Ident x, (5,4--6,5),
                    { SeparatorRange = None }), (4,0--6,5)), (4,0--6,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

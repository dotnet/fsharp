ImplFile
  (ParsedImplFileInput
     ("/root/Expression/LetIn 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Sequential
                   (SuppressNeither, true,
                    LetOrUse
                      { IsRecursive = false
                        Bindings =
                         [SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named (SynIdent (a, None), false, None, (4,8--4,9)),
                             None, Const (Int32 1, (4,12--4,13)), (4,8--4,9),
                             Yes (4,4--4,13),
                             { LeadingKeyword = Let (4,4--4,7)
                               InlineKeyword = None
                               EqualsRange = Some (4,10--4,11) })]
                        Body =
                         LetOrUse
                           { IsRecursive = false
                             Bindings =
                              [SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((4,17), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (None,
                                     SynValInfo
                                       ([], SynArgInfo ([], false, None)), None),
                                  Named
                                    (SynIdent (b, None), false, None,
                                     (4,21--4,22)), None,
                                  Const (Int32 2, (4,25--4,26)), (4,21--4,22),
                                  Yes (4,17--4,26),
                                  { LeadingKeyword = Let (4,17--4,20)
                                    InlineKeyword = None
                                    EqualsRange = Some (4,23--4,24) })]
                             Body = Ident c
                             Range = (4,17--4,31)
                             Trivia = { InKeyword = Some (4,27--4,29) }
                             IsFromSource = true }
                        Range = (4,4--4,31)
                        Trivia = { InKeyword = Some (4,14--4,16) }
                        IsFromSource = true }, Ident d, (4,4--5,5),
                    { SeparatorRange = None }), (3,0--5,5)), (3,0--5,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

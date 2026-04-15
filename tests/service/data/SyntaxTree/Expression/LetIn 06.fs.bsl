ImplFile
  (ParsedImplFileInput
     ("/root/Expression/LetIn 06.fs", false, QualifiedNameOfFile Module, [],
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
                         IfThenElse
                           (Const (Bool true, (4,20--4,24)), Ident b,
                            Some
                              (LetOrUse
                                 { IsRecursive = false
                                   Bindings =
                                    [SynBinding
                                       (None, Normal, false, false, [],
                                        PreXmlDoc ((4,37), FSharp.Compiler.Xml.XmlDocCollector),
                                        SynValData
                                          (None,
                                           SynValInfo
                                             ([], SynArgInfo ([], false, None)),
                                           None),
                                        Named
                                          (SynIdent (c, None), false, None,
                                           (4,41--4,42)), None,
                                        Const (Int32 2, (4,45--4,46)),
                                        (4,41--4,42), Yes (4,37--4,46),
                                        { LeadingKeyword = Let (4,37--4,40)
                                          InlineKeyword = None
                                          EqualsRange = Some (4,43--4,44) })]
                                   Body = Const (Int32 3, (4,50--4,51))
                                   Range = (4,37--4,51)
                                   Trivia = { InKeyword = Some (4,47--4,49) }
                                   IsFromSource = true }), Yes (4,17--4,29),
                            false, (4,17--4,51),
                            { IfKeyword = (4,17--4,19)
                              IsElif = false
                              ThenKeyword = (4,25--4,29)
                              ElseKeyword = Some (4,32--4,36)
                              IfToThenRange = (4,17--4,29) })
                        Range = (4,4--4,51)
                        Trivia = { InKeyword = Some (4,14--4,16) }
                        IsFromSource = true }, Ident d, (4,4--5,5),
                    { SeparatorRange = None }), (3,0--5,5)), (3,0--5,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

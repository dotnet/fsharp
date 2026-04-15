ImplFile
  (ParsedImplFileInput
     ("/root/Expression/LetIn 09.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (3,6--3,8)), (3,6--3,8))], None,
                     (3,4--3,8)), None,
                  Sequential
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
                              Named
                                (SynIdent (x, None), false, None, (4,8--4,9)),
                              None, Const (Int32 1, (4,12--4,13)), (4,8--4,9),
                              Yes (4,4--4,13),
                              { LeadingKeyword = Let (4,4--4,7)
                                InlineKeyword = None
                                EqualsRange = Some (4,10--4,11) })]
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
                                   (4,19--4,20)), Ident x, (4,17--4,20)),
                             Const (Int32 1, (4,21--4,22)), (4,17--4,22))
                         Range = (4,4--4,22)
                         Trivia = { InKeyword = Some (4,14--4,16) }
                         IsFromSource = true },
                     App
                       (NonAtomic, false, Ident printfn,
                        Const
                          (String ("hello", Regular, (5,12--5,19)), (5,12--5,19)),
                        (5,4--5,19)), (4,4--5,19), { SeparatorRange = None }),
                  (3,4--3,8), NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (3,9--3,10) })],
              (3,0--5,19), { InKeyword = None })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,19), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

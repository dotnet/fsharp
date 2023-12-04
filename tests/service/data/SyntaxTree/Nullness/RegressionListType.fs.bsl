ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionListType.fs", false,
      QualifiedNameOfFile RegressionListType, [], [],
      [SynModuleOrNamespace
         ([RegressionListType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })], [], (1,9--1,13))), [],
                     [List],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,9)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([],
                            SynIdent
                              (op_Nil,
                               Some
                                 (OriginalNotationWithParen
                                    ((2,6--2,7), "[]", (2,8--2,9)))), Fields [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (2,6--2,10), { BarRange = Some (2,4--2,5) });
                         SynUnionCase
                           ([],
                            SynIdent
                              (op_ColonColon,
                               Some
                                 (OriginalNotationWithParen
                                    ((3,6--3,7), "::", (3,11--3,12)))),
                            Fields
                              [SynField
                                 ([], false, Some Head,
                                  Var (SynTypar (T, None, false), (3,23--3,25)),
                                  false,
                                  PreXmlDoc ((3,17), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (3,17--3,25), { LeadingKeyword = None
                                                        MutableKeyword = None });
                               SynField
                                 ([], false, Some Tail,
                                  App
                                    (LongIdent
                                       (SynLongIdent ([list], [], [None])), None,
                                     [Var
                                        (SynTypar (T, None, false), (3,34--3,36))],
                                     [], None, true, (3,34--3,41)), false,
                                  PreXmlDoc ((3,28), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (3,28--3,41), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,6--3,41), { BarRange = Some (3,4--3,5) })],
                        (2,4--3,41)), (2,4--3,41)), [], None, (1,5--3,41),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,14--1,15)
                    WithKeyword = None })], (1,0--3,41))], PreXmlDocEmpty, [],
          None, (1,0--3,41), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

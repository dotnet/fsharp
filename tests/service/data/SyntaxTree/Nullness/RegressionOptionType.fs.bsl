ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionOptionType.fs", false,
      QualifiedNameOfFile RegressionOptionType, [], [],
      [SynModuleOrNamespace
         ([RegressionOptionType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })], [], (1,11--1,15))),
                     [], [Option],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,11)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (None, None),
                            FullType
                              (App
                                 (LongIdent
                                    (SynLongIdent ([option], [], [None])), None,
                                  [Var (SynTypar (T, None, false), (2,18--2,20))],
                                  [], None, true, (2,18--2,27)),
                               SynValInfo ([], SynArgInfo ([], false, None))),
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (2,6--2,27), { BarRange = Some (2,4--2,5) });
                         SynUnionCase
                           ([], SynIdent (Some, None),
                            FullType
                              (Fun
                                 (SignatureParameter
                                    ([], false, Some Value,
                                     Var
                                       (SynTypar (T, None, false), (3,18--3,20)),
                                     (3,12--3,20)),
                                  App
                                    (LongIdent
                                       (SynLongIdent ([option], [], [None])),
                                     None,
                                     [Var
                                        (SynTypar (T, None, false), (3,24--3,26))],
                                     [], None, true, (3,24--3,33)), (3,12--3,33),
                                  { ArrowRange = (3,21--3,23) }),
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some Value)]],
                                  SynArgInfo ([], false, None))),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,6--3,33), { BarRange = Some (3,4--3,5) })],
                        (2,4--3,33)), (2,4--3,33)), [], None, (1,5--3,33),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,16--1,17)
                    WithKeyword = None })], (1,0--3,33))], PreXmlDocEmpty, [],
          None, (1,0--3,34), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(2,6)-(2,27) parse warning This construct is deprecated: it is only for use in the F# library
(3,6)-(3,33) parse warning This construct is deprecated: it is only for use in the F# library

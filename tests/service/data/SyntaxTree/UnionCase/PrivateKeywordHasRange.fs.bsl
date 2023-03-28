ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/PrivateKeywordHasRange.fs", false,
      QualifiedNameOfFile PrivateKeywordHasRange, [], [],
      [SynModuleOrNamespace
         ([PrivateKeywordHasRange], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Currency],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,13)),
                  Simple
                    (Union
                       (Some (Private (7,4--7,11)),
                        [SynUnionCase
                           ([], SynIdent (Code, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((9,14), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (9,14--9,20), { LeadingKeyword = None })],
                            PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (9,6--9,20), { BarRange = Some (9,4--9,5) })],
                        (7,4--9,20)), (7,4--9,20)), [], None, (2,5--9,20),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,14--2,15)
                    WithKeyword = None })], (2,0--9,20))], PreXmlDocEmpty, [],
          None, (2,0--10,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives =
         [If (Not (Ident "FABLE_COMPILER"), (6,0--6,19)); EndIf (8,0--8,6)]
        CodeComments = [LineComment (3,4--3,67); LineComment (4,4--4,56)] },
      set []))

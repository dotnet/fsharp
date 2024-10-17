ImplFile
  (ParsedImplFileInput
     ("/root/Type/Interface 07.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Interface
                        (FromParseError (4,13--4,13), None, None, (4,4--4,13));
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (P, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (5,4--5,19),
                            { LeadingKeyword = Abstract (5,4--5,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (5,4--5,19),
                         { GetSetKeywords = None })], (4,4--5,19)), [], None,
                  (3,5--5,19), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,7--3,8)
                                 WithKeyword = None })], (3,0--5,19))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,19), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,14)-(5,4) parse error Incomplete structured construct at or before this point in member definition
(6,4)-(6,7) parse error Unexpected keyword 'end' in type definition. Expected incomplete structured construct at or before this point or other token.
(8,0)-(8,1) parse error Unexpected symbol '(' in implementation file

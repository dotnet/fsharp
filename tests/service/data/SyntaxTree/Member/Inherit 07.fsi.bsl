SigFile
  (ParsedSigFileInput
     ("/root/Member/Inherit 07.fsi", QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Inherit (FromParseError (4,11--4,11), (4,4--4,11));
                      Member
                        (SynValSig
                           ([], SynIdent (P, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (6,4--6,17),
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (6,4--6,17),
                         { GetSetKeywords = None })], (4,4--6,17)), [],
                  (3,5--6,17), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,7--3,8)
                                 WithKeyword = None })], (3,0--6,17))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,17), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,12)-(6,4) parse error Incomplete structured construct at or before this point in member signature

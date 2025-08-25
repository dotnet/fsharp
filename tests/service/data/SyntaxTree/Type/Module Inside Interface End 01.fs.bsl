ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Interface End 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [IFace],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,10)),
                  ObjectModel
                    (Interface,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (F, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (6,21--6,31), { ArrowRange = (6,25--6,27) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (6,8--6,31),
                            { LeadingKeyword = Abstract (6,8--6,16)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (6,8--6,31),
                         { GetSetKeywords = None })], (5,4--6,31)), [], None,
                  (4,5--6,31), { LeadingKeyword = Type (4,0--4,4)
                                 EqualsRange = Some (4,11--4,12)
                                 WithKeyword = None })], (4,0--6,31))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,31), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,60)] }, set []))

(5,4)-(5,13) parse error Unmatched 'class', 'interface' or 'struct'
(7,8)-(7,14) parse error Unexpected keyword 'module' in member definition
(9,4)-(9,7) parse error Incomplete structured construct at or before this point in definition. Expected incomplete structured construct at or before this point or other token.

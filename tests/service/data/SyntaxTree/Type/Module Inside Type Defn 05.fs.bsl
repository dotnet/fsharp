ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 05.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [IFace],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,10)),
                  ObjectModel
                    (Interface,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (F, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (5,21--5,31), { ArrowRange = (5,25--5,27) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (5,8--5,31),
                            { LeadingKeyword = Abstract (5,8--5,16)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (5,8--5,31),
                         { GetSetKeywords = None })], (4,4--5,31)), [], None,
                  (3,5--5,31), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,11--3,12)
                                 WithKeyword = None })], (3,0--5,31))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,31), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,13) parse error Unmatched 'class', 'interface' or 'struct'
(6,8)-(6,14) parse error Unexpected keyword 'module' in member definition
(8,4)-(8,7) parse error Incomplete structured construct at or before this point in definition. Expected incomplete structured construct at or before this point or other token.

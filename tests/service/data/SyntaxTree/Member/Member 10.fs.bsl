ImplFile
  (ParsedImplFileInput
     ("/root/Member/Member 10.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [U],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,7), { BarRange = Some (4,4--4,5) })],
                        (4,4--4,7)), (4,4--4,7)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]; []],
                               SynArgInfo ([], false, None)), None),
                         Wild (6,11--6,12), None,
                         ArbitraryAfterError ("memberCore2", (6,12--6,12)),
                         (6,11--6,12), NoneAtInvisible,
                         { LeadingKeyword = Member (6,4--6,10)
                           InlineKeyword = None
                           EqualsRange = None }), (6,4--6,12))], None,
                  (3,5--6,12), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,7--3,8)
                                 WithKeyword = None })], (3,0--6,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(7,0)-(7,0) parse error Expecting member body

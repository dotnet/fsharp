ImplFile
  (ParsedImplFileInput
     ("/root/Member/Static 01.fs", false, QualifiedNameOfFile Module, [], [],
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
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            Paren (Const (Unit, (7,7--7,9)), (7,7--7,9)), None,
                            ArbitraryAfterError ("memberCore2", (7,9--7,9)),
                            (7,7--7,9), NoneAtInvisible,
                            { LeadingKeyword = Static (5,4--5,10)
                              InlineKeyword = None
                              EqualsRange = None }), (5,4--7,9))], (5,4--7,9)),
                  [], None, (3,5--7,9), { LeadingKeyword = Type (3,0--3,4)
                                          EqualsRange = Some (3,7--3,8)
                                          WithKeyword = None })], (3,0--7,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,11)-(7,4) parse error Incomplete structured construct at or before this point in type definition
(8,0)-(8,0) parse error Incomplete structured construct at or before this point in member definition. Expected 'with', '=' or other token.
(8,0)-(8,0) parse error Incomplete structured construct at or before this point in definition. Expected incomplete structured construct at or before this point or other token.

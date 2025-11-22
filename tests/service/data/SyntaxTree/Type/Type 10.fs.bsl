ImplFile
  (ParsedImplFileInput
     ("/root/Type/Type 10.fs", false, QualifiedNameOfFile Type 10, [],
      [SynModuleOrNamespace
         ([N], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [M],
                 PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,0--6,8)), false,
              [Types
                 ([SynTypeDefn
                     (SynComponentInfo
                        ([], None, [], [],
                         PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (7,9--7,10)),
                      ObjectModel
                        (Class,
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((8,15), FSharp.Compiler.Xml.XmlDocCollector),
                                SynValData
                                  (Some
                                     { IsInstance = false
                                       IsDispatchSlot = false
                                       IsOverrideOrExplicitImpl = false
                                       IsFinal = false
                                       GetterOrSetterIsCompilerGenerated = false
                                       MemberKind = Member },
                                   SynValInfo
                                     ([[]], SynArgInfo ([], false, None)), None),
                                LongIdent
                                  (SynLongIdent ([M], [], [None]), None, None,
                                   Pats
                                     [Paren
                                        (Const (Unit, (8,30--8,32)),
                                         (8,30--8,32))], None, (8,29--8,32)),
                                None, Const (Int32 11, (8,35--8,37)),
                                (8,29--8,32), NoneAtInvisible,
                                { LeadingKeyword =
                                   StaticMember ((8,15--8,21), (8,22--8,28))
                                  InlineKeyword = None
                                  EqualsRange = Some (8,33--8,34) }),
                             (8,15--8,37))], (7,13--9,16)), [], None,
                      (7,9--9,16), { LeadingKeyword = Type (7,4--7,8)
                                     EqualsRange = Some (7,11--7,12)
                                     WithKeyword = None })], (7,4--9,16))],
              false, (6,0--9,16), { ModuleKeyword = Some (6,0--6,6)
                                    EqualsRange = Some (6,9--6,10) })],
          PreXmlDocEmpty, [], None, (4,0--9,16),
          { LeadingKeyword = Namespace (4,0--4,9) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,25)] }, set []))

(7,9)-(7,10) parse error Unexpected character '�' in type name

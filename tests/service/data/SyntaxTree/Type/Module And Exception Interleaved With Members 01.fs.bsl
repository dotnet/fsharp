ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module And Exception Interleaved With Members 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Interleaved],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,16)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                            LongIdent
                              (SynLongIdent
                                 ([_; X], [(5,12--5,13)], [None; None]), None,
                               None, Pats [], None, (5,11--5,14)), None,
                            Const (Int32 1, (5,17--5,18)), (5,11--5,14),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,4--5,10)
                              InlineKeyword = None
                              EqualsRange = Some (5,15--5,16) }), (5,4--5,18))],
                     (5,4--5,18)), [], None, (4,5--5,18),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,17--4,18)
                    WithKeyword = None })], (4,0--5,18));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M1],
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,13)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (y, None), false, None, (7,12--7,13)),
                      None, Const (Int32 2, (7,16--7,17)), (7,12--7,13),
                      Yes (7,8--7,17), { LeadingKeyword = Let (7,8--7,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (7,14--7,15) })],
                  (7,8--7,17), { InKeyword = None })], false, (6,4--7,17),
              { ModuleKeyword = Some (6,4--6,10)
                EqualsRange = Some (6,14--6,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,57)] }, set []))

(6,4)-(6,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(8,4)-(8,10) parse error Unexpected keyword 'member' in definition. Expected incomplete structured construct at or before this point or other token.
(9,4)-(9,13) parse error Exceptions must be defined at module level, not inside types.

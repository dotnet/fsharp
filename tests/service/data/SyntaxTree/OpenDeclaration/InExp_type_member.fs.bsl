ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_type_member.fs", false,
      QualifiedNameOfFile InExp_type_member, [],
      [SynModuleOrNamespace
         ([InExp_type_member], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (1,8--1,10)), None,
                         PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (1,5--1,8), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; PrintHello], [(2,22--2,23)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (2,33--2,35)), (2,33--2,35))],
                               Some (Public (2,11--2,17)), (2,11--2,35)), None,
                            Open
                              (ModuleOrNamespace
                                 (SynLongIdent ([System], [], [None]),
                                  (3,13--3,19)), (3,8--3,19), (3,8--4,35),
                               App
                                 (Atomic, false,
                                  LongIdent
                                    (false,
                                     SynLongIdent
                                       ([Console; WriteLine], [(4,15--4,16)],
                                        [None; None]), None, (4,8--4,25)),
                                  Paren
                                    (Const
                                       (String ("Hello!", Regular, (4,26--4,34)),
                                        (4,26--4,34)), (4,25--4,26),
                                     Some (4,34--4,35), (4,25--4,35)),
                                  (4,8--4,35))), (2,11--2,35), NoneAtInvisible,
                            { LeadingKeyword = Member (2,4--2,10)
                              InlineKeyword = None
                              EqualsRange = Some (2,36--2,37) }), (2,4--4,35))],
                     (2,4--4,35)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,8--1,10)), None,
                        PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,8), { AsKeyword = None })), (1,5--4,35),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,11--1,12)
                    WithKeyword = None })], (1,0--4,35))], PreXmlDocEmpty, [],
          None, (1,0--4,35), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

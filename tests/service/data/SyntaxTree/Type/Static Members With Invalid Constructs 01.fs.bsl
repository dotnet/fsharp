ImplFile
  (ParsedImplFileInput
     ("/root/Type/Static Members With Invalid Constructs 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,11)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,11--4,13)), None,
                         PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,11), { AsKeyword = None });
                      Member
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
                            LongIdent
                              (SynLongIdent ([StaticMethod], [], [None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,30--5,32)), (5,30--5,32))],
                               None, (5,18--5,32)), None,
                            Const (Int32 42, (5,35--5,37)), (5,18--5,32),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((5,4--5,10), (5,11--5,17))
                              InlineKeyword = None
                              EqualsRange = Some (5,33--5,34) }), (5,4--5,37));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([StaticProperty], [], [None]), None,
                               None, Pats [], None, (6,18--6,32)), None,
                            Const
                              (String ("hello", Regular, (6,35--6,42)),
                               (6,35--6,42)), (6,18--6,32), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((6,4--6,10), (6,11--6,17))
                              InlineKeyword = None
                              EqualsRange = Some (6,33--6,34) }), (6,4--6,42))],
                     (5,4--6,42)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,11--4,13)), None,
                        PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,11), { AsKeyword = None })), (4,5--6,42),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,14--4,15)
                    WithKeyword = None })], (4,0--6,42));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,4--8,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (9,12--9,13)),
                      None, Const (Int32 1, (9,16--9,17)), (9,12--9,13),
                      Yes (9,8--9,17), { LeadingKeyword = Let (9,8--9,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (9,14--9,15) })],
                  (9,8--9,17))], false, (8,4--9,17),
              { ModuleKeyword = Some (8,4--8,10)
                EqualsRange = Some (8,25--8,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--9,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,52)] }, set []))

(8,4)-(8,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(11,4)-(11,10) parse error Unexpected keyword 'static' in definition. Expected incomplete structured construct at or before this point or other token.

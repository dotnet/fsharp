ImplFile
  (ParsedImplFileInput
     ("/root/Type/Inherit With Invalid Constructs 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Base],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,9)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,9--4,11)), None,
                         PreXmlDoc ((4,9), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,9), { AsKeyword = None });
                      Member
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
                                 ([_; BaseMethod], [(5,12--5,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,23--5,25)), (5,23--5,25))],
                               None, (5,11--5,25)), None,
                            Const (Int32 1, (5,28--5,29)), (5,11--5,25),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,4--5,10)
                              InlineKeyword = None
                              EqualsRange = Some (5,26--5,27) }), (5,4--5,29))],
                     (5,4--5,29)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,9--4,11)), None,
                        PreXmlDoc ((4,9), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,9), { AsKeyword = None })), (4,5--5,29),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,12--4,13)
                    WithKeyword = None })], (4,0--5,29));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Derived],
                     PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (7,5--7,12)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (7,12--7,14)), None,
                         PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (7,5--7,12), { AsKeyword = None });
                      ImplicitInherit
                        (LongIdent (SynLongIdent ([Base], [], [None])),
                         Const (Unit, (8,16--8,18)), None, (8,4--8,18),
                         { InheritKeyword = (8,4--8,11) })], (8,4--8,18)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (7,12--7,14)), None,
                        PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (7,5--7,12), { AsKeyword = None })), (7,5--8,18),
                  { LeadingKeyword = Type (7,0--7,4)
                    EqualsRange = Some (7,15--7,16)
                    WithKeyword = None })], (7,0--8,18));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (10,4--10,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((11,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (11,12--11,13)),
                      None, Const (Int32 2, (11,16--11,17)), (11,12--11,13),
                      Yes (11,8--11,17), { LeadingKeyword = Let (11,8--11,11)
                                           InlineKeyword = None
                                           EqualsRange = Some (11,14--11,15) })],
                  (11,8--11,17))], false, (10,4--11,17),
              { ModuleKeyword = Some (10,4--10,10)
                EqualsRange = Some (10,25--10,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--11,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,45)] }, set []))

(10,4)-(10,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(13,4)-(13,12) parse error Unexpected keyword 'override' in definition. Expected incomplete structured construct at or before this point or other token.

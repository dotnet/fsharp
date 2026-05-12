ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Between Constructors 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyClass],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,12)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         Paren
                           (Typed
                              (Named
                                 (SynIdent (x, None), false, None, (4,13--4,14)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (4,13--4,19)), (4,12--4,20)), None,
                         PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,12), { AsKeyword = None });
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
                                      MemberKind = Constructor },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([new], [], [None]), None,
                               Some (SynValTyparDecls (None, false)),
                               Pats
                                 [Paren (Const (Unit, (5,7--5,9)), (5,7--5,9))],
                               None, (5,4--5,7)), None,
                            App
                              (Atomic, false, Ident MyClass,
                               Paren
                                 (Const (Int32 0, (5,20--5,21)), (5,19--5,20),
                                  Some (5,21--5,22), (5,19--5,22)), (5,12--5,22)),
                            (5,4--5,9), NoneAtInvisible,
                            { LeadingKeyword = New (5,4--5,7)
                              InlineKeyword = None
                              EqualsRange = Some (5,10--5,11) }), (5,4--5,22))],
                     (5,4--5,22)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (4,13--4,14)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,13--4,19)), (4,12--4,20)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--5,22),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,21--4,22)
                    WithKeyword = None })], (4,0--5,22));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,4--7,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (8,12--8,13)),
                      None, Const (Int32 1, (8,16--8,17)), (8,12--8,13),
                      Yes (8,8--8,17), { LeadingKeyword = Let (8,8--8,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (8,14--8,15) })],
                  (8,8--8,17), { InKeyword = None })], false, (7,4--8,17),
              { ModuleKeyword = Some (7,4--7,10)
                EqualsRange = Some (7,25--7,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,39)] }, set []))

(7,4)-(7,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(10,4)-(10,7) parse error Unexpected keyword 'new' in definition. Expected incomplete structured construct at or before this point or other token.

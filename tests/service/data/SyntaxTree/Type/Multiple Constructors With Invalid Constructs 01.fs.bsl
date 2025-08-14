ImplFile
  (ParsedImplFileInput
     ("/root/Type/Multiple Constructors With Invalid Constructs 01.fs", false,
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
                                 (SynIdent (primary, None), false, None,
                                  (4,13--4,20)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (4,13--4,25)), (4,12--4,26)), None,
                         PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,12), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, true, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (value, None), false, None,
                                (5,16--5,21)), None, Ident primary, (5,16--5,21),
                             Yes (5,4--5,31),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,22--5,23) })], false, false,
                         (5,4--5,31));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 [Paren (Const (Unit, (7,7--7,9)), (7,7--7,9))],
                               None, (7,4--7,7)), None,
                            App
                              (Atomic, false, Ident MyClass,
                               Paren
                                 (Const (Int32 0, (7,20--7,21)), (7,19--7,20),
                                  Some (7,21--7,22), (7,19--7,22)), (7,12--7,22)),
                            (7,4--7,9), NoneAtInvisible,
                            { LeadingKeyword = New (7,4--7,7)
                              InlineKeyword = None
                              EqualsRange = Some (7,10--7,11) }), (7,4--7,22))],
                     (5,4--7,22)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        Paren
                          (Typed
                             (Named
                                (SynIdent (primary, None), false, None,
                                 (4,13--4,20)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,13--4,25)), (4,12--4,26)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--7,22),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,27--4,28)
                    WithKeyword = None })], (4,0--7,22));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (9,4--9,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((10,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (10,12--10,13)),
                      None, Const (Int32 1, (10,16--10,17)), (10,12--10,13),
                      Yes (10,8--10,17), { LeadingKeyword = Let (10,8--10,11)
                                           InlineKeyword = None
                                           EqualsRange = Some (10,14--10,15) })],
                  (10,8--10,17))], false, (9,4--10,17),
              { ModuleKeyword = Some (9,4--9,10)
                EqualsRange = Some (9,25--9,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--10,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,52)] }, set []))

(9,4)-(9,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(12,4)-(12,7) parse error Unexpected keyword 'new' in definition. Expected incomplete structured construct at or before this point or other token.

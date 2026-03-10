ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Class With Constructor 01.fs", false,
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
                         (4,5--4,12), { AsKeyword = None })], (4,5--4,22)), [],
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
                        (4,5--4,12), { AsKeyword = None })), (4,5--4,22),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,21--4,22)
                    WithKeyword = None })], (4,0--4,22));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InternalModule],
                 PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,4--5,25)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (helper, None), false, None, (6,12--6,18)),
                      None, Const (Int32 42, (6,21--6,23)), (6,12--6,18),
                      Yes (6,8--6,23), { LeadingKeyword = Let (6,8--6,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (6,19--6,20) })],
                  (6,8--6,23), { InKeyword = None })], false, (5,4--6,23),
              { ModuleKeyword = Some (5,4--5,10)
                EqualsRange = Some (5,26--5,27) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,23), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,48)] }, set []))

(5,4)-(5,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(4,5)-(4,13) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.

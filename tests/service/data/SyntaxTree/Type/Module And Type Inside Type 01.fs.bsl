ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module And Type Inside Type 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,11)),
                  Simple (None (4,5--4,13), (4,5--4,13)), [], None, (4,5--4,13),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,12--4,13)
                    WithKeyword = None })], (4,0--4,13));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,4--5,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (helper, None), false, None, (6,12--6,18)),
                      None, Const (Int32 10, (6,21--6,23)), (6,12--6,18),
                      Yes (6,8--6,23), { LeadingKeyword = Let (6,8--6,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (6,19--6,20) })],
                  (6,8--6,23), { InKeyword = None })], false, (5,4--6,23),
              { ModuleKeyword = Some (5,4--5,10)
                EqualsRange = Some (5,25--5,26) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [InvalidType],
                     PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (8,9--8,20)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([string], [], [None])),
                        (8,23--8,29)), (8,23--8,29)), [], None, (8,9--8,29),
                  { LeadingKeyword = Type (8,4--8,8)
                    EqualsRange = Some (8,21--8,22)
                    WithKeyword = None })], (8,4--8,29))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,29), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,54)] }, set []))

(5,4)-(5,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(4,5)-(4,13) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
(8,4)-(8,8) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.

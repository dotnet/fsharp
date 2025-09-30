ImplFile
  (ParsedImplFileInput
     ("/root/Type/Cascading Nested Invalid Constructs 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple (None (4,5--4,8), (4,5--4,8)), [], None, (4,5--4,8),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--4,8));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,9--5,10)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (5,13--5,16)), (5,13--5,16)), [], None, (5,9--5,16),
                  { LeadingKeyword = Type (5,4--5,8)
                    EqualsRange = Some (5,11--5,12)
                    WithKeyword = None })], (5,4--5,16));
           NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,8--6,16)), false,
              [Expr (Const (Unit, (6,19--6,21)), (6,19--6,21))], false,
              (6,8--6,21), { ModuleKeyword = Some (6,8--6,14)
                             EqualsRange = Some (6,17--6,18) });
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (D, None), Fields [], PreXmlDocEmpty, None,
                       (7,22--7,23), { BarRange = None }), None,
                    PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (7,12--7,23)), None, [], (7,12--7,23)), (7,12--7,23))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,23), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,47)] }, set []))

(5,4)-(5,8) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.
(4,5)-(4,8) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
(6,8)-(6,14) parse error Modules cannot be nested inside types. Define modules at module or namespace level.

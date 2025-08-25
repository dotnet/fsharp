ImplFile
  (ParsedImplFileInput
     ("/root/Type/Double Semicolon Delimiters 01.fs", false,
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
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,12)), false,
              [Expr (Const (Unit, (6,15--6,17)), (6,15--6,17))], false,
              (6,4--6,17), { ModuleKeyword = Some (6,4--6,10)
                             EqualsRange = Some (6,13--6,14) });
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (D, None), Fields [], PreXmlDocEmpty, None,
                       (7,14--7,15), { BarRange = None }), None,
                    PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (7,4--7,15)), None, [], (7,4--7,15)), (7,4--7,15))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,15), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,63)] }, set []))

(5,4)-(5,8) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.
(4,5)-(4,8) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.

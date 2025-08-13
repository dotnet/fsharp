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
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,7), { BarRange = Some (5,4--5,5) })],
                        (5,4--5,7)), (5,4--5,7)), [], None, (4,5--5,7),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--5,7));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,9--6,10)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([A], [], [None])),
                        (6,13--6,14)), (6,13--6,14)), [], None, (6,9--6,14),
                  { LeadingKeyword = Type (6,4--6,8)
                    EqualsRange = Some (6,11--6,12)
                    WithKeyword = None })], (6,4--6,14));
           NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,8--7,16)), false,
              [Expr (Const (Unit, (7,19--7,21)), (7,19--7,21))], false,
              (7,8--7,21), { ModuleKeyword = Some (7,8--7,14)
                             EqualsRange = Some (7,17--7,18) });
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (D, None), Fields [], PreXmlDocEmpty, None,
                       (8,22--8,23), { BarRange = None }), None,
                    PreXmlDoc ((8,12), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (8,12--8,23)), None, [], (8,12--8,23)), (8,12--8,23));
           ModuleAbbrev (E, [C], (9,16--9,28));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((10,20), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (10,26--10,28)), (10,26--10,28))],
                     None, (10,24--10,28)), None, Const (Unit, (10,31--10,33)),
                  (10,24--10,28), NoneAtLet,
                  { LeadingKeyword = Let (10,20--10,23)
                    InlineKeyword = None
                    EqualsRange = Some (10,29--10,30) })], (10,20--10,33));
           Open
             (ModuleOrNamespace
                (SynLongIdent ([System], [], [None]), (11,29--11,35)),
              (11,24--11,35));
           NestedModule
             (SynComponentInfo
                ([], None, [], [G],
                 PreXmlDoc ((12,28), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (12,28--12,36)), false,
              [ModuleAbbrev (H, [E], (13,32--13,44))], false, (12,28--13,44),
              { ModuleKeyword = Some (12,28--12,34)
                EqualsRange = Some (12,37--12,38) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--13,44), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,50)] }, set []))

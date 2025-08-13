ImplFile
  (ParsedImplFileInput
     ("/root/Type/Same Line Declarations 01.fs", false,
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
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([A], [], [None])),
                        (4,9--4,10)), (4,9--4,10)), [], None, (4,5--4,10),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--4,10));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,16--4,17)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([A], [], [None])),
                        (4,20--4,21)), (4,20--4,21)), [], None, (4,16--4,21),
                  { LeadingKeyword = Type (4,11--4,15)
                    EqualsRange = Some (4,18--4,19)
                    WithKeyword = None })], (4,11--4,21));
           NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((4,22), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,22--4,30)), false,
              [Types
                 ([SynTypeDefn
                     (SynComponentInfo
                        ([], None, [], [CC],
                         PreXmlDoc ((4,33), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (4,38--4,40)),
                      Simple
                        (TypeAbbrev
                           (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                            (4,43--4,46)), (4,43--4,46)), [], None, (4,38--4,46),
                      { LeadingKeyword = Type (4,33--4,37)
                        EqualsRange = Some (4,41--4,42)
                        WithKeyword = None })], (4,33--4,46))], false,
              (4,22--4,46), { ModuleKeyword = Some (4,22--4,28)
                              EqualsRange = Some (4,31--4,32) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--4,46), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,68)] }, set []))

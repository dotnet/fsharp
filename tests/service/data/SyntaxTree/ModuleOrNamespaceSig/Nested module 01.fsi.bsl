SigFile
  (ParsedSigFileInput
     ("/root/ModuleOrNamespaceSig/Nested module 01.fsi",
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [A],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false,
              [Val
                 (SynValSig
                    ([], SynIdent (a, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                     Single None, None, (4,4--4,14),
                     { LeadingKeyword = Val (4,4--4,7)
                       InlineKeyword = None
                       WithKeyword = None
                       EqualsRange = None }), (4,4--4,14))], (3,0--4,14),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,9--3,10) });
           Val
             (SynValSig
                ([], SynIdent (b, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, None, (6,0--6,10),
                 { LeadingKeyword = Val (6,0--6,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }), (6,0--6,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,10), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

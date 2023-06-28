SigFile
  (ParsedSigFileInput
     ("/root/ModuleOrNamespaceSig/ModuleAbbreviation.fsi",
      QualifiedNameOfFile Foo, [], [],
      [SynModuleOrNamespaceSig
         ([Foo], false, NamedModule,
          [Open
             (ModuleOrNamespace
                (SynLongIdent ([System], [], [None]), (3,5--3,11)), (3,0--3,11));
           Open
             (ModuleOrNamespace
                (SynLongIdent ([System; Text], [(4,11--4,12)], [None; None]),
                 (4,5--4,16)), (4,0--4,16)); ModuleAbbrev (A, [B], (6,0--6,12));
           NestedModule
             (SynComponentInfo
                ([], None, [], [Bar],
                 PreXmlDoc ((8,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,0--8,10)), false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([], None, [], [a],
                         PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (9,9--9,10)),
                      Simple
                        (Union
                           (None,
                            [SynUnionCase
                               ([], SynIdent (Ex, None), Fields [],
                                PreXmlDoc ((10,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (10,10--10,12),
                                { BarRange = Some (10,8--10,9) });
                             SynUnionCase
                               ([], SynIdent (Why, None), Fields [],
                                PreXmlDoc ((11,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (11,10--11,13),
                                { BarRange = Some (11,8--11,9) });
                             SynUnionCase
                               ([], SynIdent (Zed, None), Fields [],
                                PreXmlDoc ((12,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (12,10--12,13),
                                { BarRange = Some (12,8--12,9) })],
                            (10,8--12,13)), (10,8--12,13)), [], (9,9--12,13),
                      { LeadingKeyword = Type (9,4--9,8)
                        EqualsRange = Some (9,11--9,12)
                        WithKeyword = None })], (9,4--12,13))], (8,0--12,13),
              { ModuleKeyword = Some (8,0--8,6)
                EqualsRange = Some (8,11--8,12) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--12,13), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

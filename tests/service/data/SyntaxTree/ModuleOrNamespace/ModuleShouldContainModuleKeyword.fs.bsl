ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/ModuleShouldContainModuleKeyword.fs", false,
      QualifiedNameOfFile FsAutoComplete.FCSPatches, [], [],
      [SynModuleOrNamespace
         ([FsAutoComplete; FCSPatches], false, NamedModule,
          [Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([FSharp; Compiler; Syntax], [(7,11--7,12); (7,20--7,21)],
                    [None; None; None]), (7,5--7,27)), (7,0--7,27));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([FSharp; Compiler; Text], [(8,11--8,12); (8,20--8,21)],
                    [None; None; None]), (8,5--8,25)), (8,0--8,25));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([FsAutoComplete; UntypedAstUtils], [(9,19--9,20)],
                    [None; None]), (9,5--9,35)), (9,0--9,35));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([FSharp; Compiler; CodeAnalysis],
                    [(10,11--10,12); (10,20--10,21)], [None; None; None]),
                 (10,5--10,33)), (10,0--10,33));
           NestedModule
             (SynComponentInfo
                ([], None, [], [SynExprAppLocationsImpl],
                 PreXmlDoc ((12,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 Some (Internal (12,7--12,15)), (12,0--12,39)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (a, None), false, None, (13,8--13,9)),
                      None, Const (Int32 42, (13,12--13,14)), (13,8--13,9),
                      Yes (13,4--13,14), { LeadingKeyword = Let (13,4--13,7)
                                           InlineKeyword = None
                                           EqualsRange = Some (13,10--13,11) })],
                  (13,4--13,14))], false, (12,0--13,14),
              { ModuleKeyword = Some (12,0--12,6)
                EqualsRange = Some (12,40--12,41) })],
          PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--13,14), { LeadingKeyword = Module (5,0--5,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

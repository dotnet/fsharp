ImplFile
  (ParsedImplFileInput
     ("/root/NestedModule/NestedModuleWithBeginEndAndDecls.fs", false,
      QualifiedNameOfFile NestedModuleWithBeginEndAndDecls, [], [],
      [SynModuleOrNamespace
         ([X], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Y],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (a, None), false, None, (5,12--5,13)),
                      None, Const (Int32 0, (5,16--5,17)), (5,12--5,13),
                      Yes (5,8--5,17), { LeadingKeyword = Let (5,8--5,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (5,14--5,15) })],
                  (5,8--5,17))], false, (3,0--6,7),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,9--3,10) })], PreXmlDocEmpty, [], None,
          (1,0--6,7), { LeadingKeyword = Namespace (1,0--1,9) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

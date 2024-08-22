SigFile
  (ParsedSigFileInput
     ("/root/NestedModule/NestedModuleWithBeginEndAndDecls.fsi",
      QualifiedNameOfFile NestedModuleWithBeginEndAndDecls, [], [],
      [SynModuleOrNamespaceSig
         ([X], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Y],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false,
              [Val
                 (SynValSig
                    ([], SynIdent (a, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                     Single None, None, (5,8--5,18),
                     { LeadingKeyword = Val (5,8--5,11)
                       InlineKeyword = None
                       WithKeyword = None
                       EqualsRange = None }), (5,8--5,18));
               Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([], None, [], [B],
                         PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (6,13--6,14)),
                      Simple
                        (TypeAbbrev
                           (Ok, LongIdent (SynLongIdent ([string], [], [None])),
                            (6,17--6,23)), (6,17--6,23)), [], (6,13--6,23),
                      { LeadingKeyword = Type (6,8--6,12)
                        EqualsRange = Some (6,15--6,16)
                        WithKeyword = None })], (6,8--6,23))], (3,0--7,7),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,9--3,10) })], PreXmlDocEmpty, [], None,
          (1,0--7,7), { LeadingKeyword = Namespace (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

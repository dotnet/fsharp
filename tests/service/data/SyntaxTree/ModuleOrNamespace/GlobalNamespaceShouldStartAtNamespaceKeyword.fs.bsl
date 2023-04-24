ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/GlobalNamespaceShouldStartAtNamespaceKeyword.fs",
      false, QualifiedNameOfFile GlobalNamespaceShouldStartAtNamespaceKeyword,
      [], [],
      [SynModuleOrNamespace
         ([], false, GlobalNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (6,9--6,12)), (6,9--6,12)), [], None, (6,5--6,12),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,7--6,8)
                    WithKeyword = None })], (6,0--6,12))], PreXmlDocEmpty, [],
          None, (4,0--6,12), { LeadingKeyword = Namespace (4,0--4,9) })],
      (true, true),
      { ConditionalDirectives = []
        CodeComments = [LineComment (2,0--2,6); LineComment (3,0--3,6)] },
      set []))

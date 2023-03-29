ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs",
      false,
      QualifiedNameOfFile
        MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword,
      [], [],
      [SynModuleOrNamespace
         ([TypeEquality], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Teq],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,8)),
                  ObjectModel (Class, [], (5,11--5,20)), [], None, (4,0--5,20),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,9--5,10)
                    WithKeyword = None })], (4,0--5,20))], PreXmlDocEmpty, [],
          None, (2,0--5,20), { LeadingKeyword = Namespace (2,0--2,9) });
       SynModuleOrNamespace
         ([Foobar], false, DeclaredNamespace,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((9,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (9,4--9,5)), None,
                  Const (Int32 42, (9,8--9,10)), (9,4--9,5), Yes (9,0--9,10),
                  { LeadingKeyword = Let (9,0--9,3)
                    InlineKeyword = None
                    EqualsRange = Some (9,6--9,7) })], (9,0--9,10))],
          PreXmlDocEmpty, [], None, (7,0--9,10),
          { LeadingKeyword = Namespace (7,0--7,9) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

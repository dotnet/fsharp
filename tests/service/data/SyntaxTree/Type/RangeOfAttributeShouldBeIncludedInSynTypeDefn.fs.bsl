ImplFile
  (ParsedImplFileInput
     ("/root/Type/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs", false,
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynTypeDefn, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSynTypeDefn], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Foo], [], [None])
                            ArgExpr = Const (Unit, (2,2--2,5))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (2,2--2,5) }]
                        Range = (2,0--2,7) }], None, [], [Bar],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  ObjectModel (Class, [], (4,4--5,7)), [], None, (2,0--5,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (2,0--5,7))], PreXmlDocEmpty, [],
          None, (2,0--6,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

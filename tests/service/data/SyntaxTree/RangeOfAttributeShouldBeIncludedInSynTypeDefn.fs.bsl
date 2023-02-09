ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs", false,
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynTypeDefn, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSynTypeDefn], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Foo], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (1,2--1,5))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (1,2--1,5) }]
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (1,0--1,7) }],
                     None, [], [Bar],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,5--2,8)),
                  ObjectModel
                    (Class, [],
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (3,4--4,7)),
                  [], None,
                  /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (1,0--4,7),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (1,0--4,7))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (1,0--4,7),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
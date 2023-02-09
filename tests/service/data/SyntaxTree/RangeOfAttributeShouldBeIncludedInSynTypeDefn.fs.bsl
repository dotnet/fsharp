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
                                /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,2--2,5))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,2--2,5) }]
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,0--2,7) }],
                     None, [], [Bar],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (3,5--3,8)),
                  ObjectModel
                    (Class, [],
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (4,4--5,7)),
                  [], None,
                  /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,0--5,7),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (3,0--3,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (3,9--3,10)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,0--5,7))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynTypeDefn.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
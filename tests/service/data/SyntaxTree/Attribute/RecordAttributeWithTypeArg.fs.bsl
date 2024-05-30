ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RecordAttributeWithTypeArg.fs", false,
      QualifiedNameOfFile RecordAttributeWithTypeArg, [], [],
      [SynModuleOrNamespace
         ([RecordAttributeWithTypeArg], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Foo], [], [None])
                            TypeArgs =
                             [LongIdent (SynLongIdent ([int], [], [None]))]
                            ArgExpr = Const (Unit, (1,2--1,5))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (1,2--1,5) }]
                        Range = (1,0--1,11) }], None, [], [Bar],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some id,
                            LongIdent (SynLongIdent ([string], [], [None])),
                            false,
                            PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,8--4,19), { LeadingKeyword = None
                                                 MutableKeyword = None })],
                        (3,4--5,5)), (3,4--5,5)), [], None, (1,0--5,5),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (1,0--5,5))], PreXmlDocEmpty, [],
          None, (1,0--5,5), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,10)-(1,11) parse error Unexpected symbol '>' in attribute list

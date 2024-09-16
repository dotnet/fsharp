ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/ExplicitField.fs", false,
      QualifiedNameOfFile ExplicitField, [], [],
      [SynModuleOrNamespace
         ([ExplicitField], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyStruct],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,13)),
                  ObjectModel
                    (Struct,
                     [ValField
                        (SynField
                           ([], false, Some myString,
                            WithNull
                              (LongIdent (SynLongIdent ([string], [], [None])),
                               false, (3,31--3,44), { BarRange = (3,38--3,39) }),
                            true,
                            PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,8--3,44),
                            { LeadingKeyword = Some (Val (3,8--3,11))
                              MutableKeyword = Some (3,12--3,19) }), (3,8--3,44))],
                     (2,4--4,7)), [], None, (1,5--4,7),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,14--1,15)
                    WithKeyword = None })], (1,0--4,7))], PreXmlDocEmpty, [],
          None, (1,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

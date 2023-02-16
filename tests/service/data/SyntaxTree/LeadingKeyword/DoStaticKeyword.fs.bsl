ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/DoStaticKeyword.fs", false,
      QualifiedNameOfFile DoStaticKeyword, [], [],
      [SynModuleOrNamespace
         ([DoStaticKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (3,11--3,16)), None,
                             Const (Unit, (3,14--3,16)), (3,11--3,16), NoneAtDo,
                             { LeadingKeyword =
                                StaticDo ((3,4--3,10), (3,11--3,13))
                               InlineKeyword = None
                               EqualsRange = None })], true, false, (3,4--3,16))],
                     (3,4--3,16)), [], None, (2,5--3,16),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,16))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

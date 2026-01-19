ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_type_let.fs", false,
      QualifiedNameOfFile InExp_type_let, [],
      [SynModuleOrNamespace
         ([InExp_type_let], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (1,8--1,10)), None,
                         PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (1,5--1,8), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Wild (2,8--2,9), None,
                             Open
                               (ModuleOrNamespace
                                  (SynLongIdent ([System], [], [None]),
                                   (3,13--3,19)), (3,8--3,19), (3,8--5,21),
                                Open
                                  (Type
                                     (LongIdent
                                        (SynLongIdent ([Console], [], [None])),
                                      (4,18--4,25)), (4,8--4,25), (4,8--5,21),
                                   App
                                     (NonAtomic, false, Ident WriteLine,
                                      Const (Int32 123, (5,18--5,21)),
                                      (5,8--5,21)))), (2,8--2,9),
                             Yes (2,4--5,21),
                             { LeadingKeyword = Let (2,4--2,7)
                               InlineKeyword = None
                               EqualsRange = Some (2,10--2,11) })], false, false,
                         (2,4--5,21))], (2,4--5,21)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,8--1,10)), None,
                        PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,8), { AsKeyword = None })), (1,5--5,21),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,11--1,12)
                    WithKeyword = None })], (1,0--5,21))], PreXmlDocEmpty, [],
          None, (1,0--5,21), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

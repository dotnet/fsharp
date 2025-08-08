ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Enum.fs", false,
      QualifiedNameOfFile InType_Enum, [],
      [SynModuleOrNamespace
         ([InType_Enum], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [AUnion],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,11)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (A, None),
                            Const (Int32 0, (2,10--2,11)),
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (2,6--2,11), { BarRange = Some (2,4--2,5)
                                           EqualsRange = (2,8--2,9) })],
                        (2,4--2,11)), (2,4--2,11)),
                  [Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (3,9--3,15)),
                      (3,4--3,15))], None, (1,5--3,15),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,12--1,13)
                    WithKeyword = None })], (1,0--3,15))], PreXmlDocEmpty, [],
          None, (1,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

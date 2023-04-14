ImplFile
  (ParsedImplFileInput
     ("/root/Member/Implicit ctor - Type - Fun 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([Typed
                               (Id (i, None, false, false, false, (3,7--3,8)),
                                Fun
                                  (LongIdent (SynLongIdent ([a], [], [None])),
                                   LongIdent (SynLongIdent ([b], [], [None])),
                                   (3,10--3,16), { ArrowRange = (3,12--3,14) }),
                                (3,7--3,16))], (3,6--3,17)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None })], (3,20--3,29)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Typed
                              (Id (i, None, false, false, false, (3,7--3,8)),
                               Fun
                                 (LongIdent (SynLongIdent ([a], [], [None])),
                                  LongIdent (SynLongIdent ([b], [], [None])),
                                  (3,10--3,16), { ArrowRange = (3,12--3,14) }),
                               (3,7--3,16))], (3,6--3,17)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--3,29),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,18--3,19)
                    WithKeyword = None })], (3,0--3,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

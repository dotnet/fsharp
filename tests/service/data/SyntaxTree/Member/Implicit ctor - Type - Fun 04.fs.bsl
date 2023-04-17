ImplFile
  (ParsedImplFileInput
     ("/root/Member/Implicit ctor - Type - Fun 04.fs", false,
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
                                   FromParseError (3,14--3,14), (3,10--3,14),
                                   { ArrowRange = (3,12--3,14) }), (3,7--3,16))],
                            (3,6--3,16)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None })], (3,19--3,28)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Typed
                              (Id (i, None, false, false, false, (3,7--3,8)),
                               Fun
                                 (LongIdent (SynLongIdent ([a], [], [None])),
                                  FromParseError (3,14--3,14), (3,10--3,14),
                                  { ArrowRange = (3,12--3,14) }), (3,7--3,16))],
                           (3,6--3,16)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--3,28),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,17--3,18)
                    WithKeyword = None })], (3,0--3,28))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,28), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,15)-(3,16) parse error Unexpected symbol ')' in type

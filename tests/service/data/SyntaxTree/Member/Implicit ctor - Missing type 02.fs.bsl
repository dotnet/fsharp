ImplFile
  (ParsedImplFileInput
     ("/root/Member/Implicit ctor - Missing type 02.fs", false,
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
                                FromParseError (3,9--3,9), (3,7--3,9));
                             Id (j, None, false, false, false, (3,11--3,12))],
                            [(3,9--3,10)], (3,6--3,13)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None })], (3,16--3,25)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Typed
                              (Id (i, None, false, false, false, (3,7--3,8)),
                               FromParseError (3,9--3,9), (3,7--3,9));
                            Id (j, None, false, false, false, (3,11--3,12))],
                           [(3,9--3,10)], (3,6--3,13)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--3,25),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,14--3,15)
                    WithKeyword = None })], (3,0--3,25))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,25), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,9)-(3,10) parse error Unexpected symbol ',' in type definition

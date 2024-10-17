ImplFile
  (ParsedImplFileInput
     ("/root/Member/Implicit ctor - Type - Fun 06.fs", false,
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
                         Paren
                           (Typed
                              (Named
                                 (SynIdent (i, None), false, None, (3,7--3,8)),
                               Fun
                                 (LongIdent (SynLongIdent ([a], [], [None])),
                                  Fun
                                    (FromParseError (3,15--3,15),
                                     Tuple
                                       (false,
                                        [Type
                                           (LongIdent
                                              (SynLongIdent ([c], [], [None])));
                                         Star (3,20--3,21);
                                         Type (FromParseError (3,21--3,21))],
                                        (3,18--3,21)), (3,15--3,21),
                                     { ArrowRange = (3,15--3,17) }),
                                  (3,10--3,21), { ArrowRange = (3,12--3,14) }),
                               (3,7--3,23)), (3,6--3,23)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None })], (3,26--3,35)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        Paren
                          (Typed
                             (Named
                                (SynIdent (i, None), false, None, (3,7--3,8)),
                              Fun
                                (LongIdent (SynLongIdent ([a], [], [None])),
                                 Fun
                                   (FromParseError (3,15--3,15),
                                    Tuple
                                      (false,
                                       [Type
                                          (LongIdent
                                             (SynLongIdent ([c], [], [None])));
                                        Star (3,20--3,21);
                                        Type (FromParseError (3,21--3,21))],
                                       (3,18--3,21)), (3,15--3,21),
                                    { ArrowRange = (3,15--3,17) }), (3,10--3,21),
                                 { ArrowRange = (3,12--3,14) }), (3,7--3,23)),
                           (3,6--3,23)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--3,35),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,24--3,25)
                    WithKeyword = None })], (3,0--3,35))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,35), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,22)-(3,23) parse error Unexpected symbol ')' in type
(3,15)-(3,17) parse error Expecting type

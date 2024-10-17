ImplFile
  (ParsedImplFileInput
     ("/root/Member/Implicit ctor - Type - Tuple 04.fs", false,
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
                               Tuple
                                 (false,
                                  [Type
                                     (LongIdent (SynLongIdent ([a], [], [None])));
                                   Star (3,12--3,13);
                                   Type
                                     (LongIdent (SynLongIdent ([b], [], [None])));
                                   Star (3,16--3,17);
                                   Type (FromParseError (3,17--3,17))],
                                  (3,10--3,17)), (3,7--3,19)), (3,6--3,19)),
                         None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None })], (3,22--3,31)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        Paren
                          (Typed
                             (Named
                                (SynIdent (i, None), false, None, (3,7--3,8)),
                              Tuple
                                (false,
                                 [Type
                                    (LongIdent (SynLongIdent ([a], [], [None])));
                                  Star (3,12--3,13);
                                  Type
                                    (LongIdent (SynLongIdent ([b], [], [None])));
                                  Star (3,16--3,17);
                                  Type (FromParseError (3,17--3,17))],
                                 (3,10--3,17)), (3,7--3,19)), (3,6--3,19)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--3,31),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,20--3,21)
                    WithKeyword = None })], (3,0--3,31))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,31), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,18)-(3,19) parse error Unexpected symbol ')' in type

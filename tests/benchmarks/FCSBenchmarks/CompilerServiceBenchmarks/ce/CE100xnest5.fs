module SomeModule =

    type StrChainBuilder() =
        member this.Zero() = ""
        member this.Delay(f) = f ()
        member this.Yield(x: string) = x
        member this.Combine(a, b) = a + b

    let strchain = StrChainBuilder()

    // 100 x nesting of 5
    let test =
        strchain {
            "test0"
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
            strchain {
                "test1"

                strchain {
                    "test2"

                    strchain {
                        "test3"

                        strchain {
                            "test4"
                            strchain { "test5" }
                        }
                    }
                }
            }
        }

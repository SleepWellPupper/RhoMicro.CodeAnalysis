## Release 23.0.0-rc0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
 RMJ0001 | Usage    | Warning  | `ToStringSetting` is ignored due to user defined `ToString` implementation                                    
 RMJ0002 | Usage    | Error    | `record` unions are disallowed                                                                                
 RMJ0003 | Usage    | Error    | Generic unions cannot be json serializable                                                                    
 RMJ0004 | Usage    | Error    | No more than 31 variant groups may be defined                                                                 
 RMJ0005 | Usage    | Error    | `static` unions are disallowed                                                                                
 RMJ0006 | Usage    | Error    | Duplicate variant names are disallowed                                                                        
 RMJ0007 | Design   | Warning  | At least one unmanaged or managed struct or nullable reference type variant must be defined for struct unions 
 RMJ0008 | Design   | Warning  | `interface` variants are excluded from conversion operators                                                   
 RMJ0009 | Usage    | Error    | Duplicate variant types are disallowed                                                                        
 RMJ0010 | Usage    | Error    | `object` cannot be used as a variant                                                                          
 RMJ0012 | Usage    | Error    | Variants that are the union itself are disallowed                                                             
 RMJ0013 | Usage    | Error    | Explicitly defined base classes are disallowed                                                                
 RMJ0014 | Usage    | Warning  | Prefer `Nullable<T>` over `IsNullable = true`                                                                 
 RMJ0015 | Usage    | Error    | `Nullable<T>` and `T` variants for the same type `T` are disallowed                                           
 RMJ0016 | Usage    | Warning  | `UnionTypeSettings` are ignored if missing `UnionTypeAttribute`                                               
 RMJ0017 | Usage    | Warning  | Duplicate variant group names are ignored                                                                     
 RMJ0018 | Design   | Warning  | Class unions should be sealed                                                                                 
 RMJ0019 | Usage    | Error    | `ValueType` cannot be used as a variant of struct unions                                                      
 RMJ0020 | Usage    | Error    | `ref struct` cannot be union                                                                                  

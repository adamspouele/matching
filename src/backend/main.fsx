#r "../../Suave.2.4.3/lib/net461/Suave.dll"

open Suave 
open Suave.Json
open System.Runtime.Serialization
open Suave.Filters
open Suave.Operators
open Suave.Successful


//

let app =
  choose
    [ GET >=> choose
        [ 
          path "/hello" >=> OK "Hello GET"
          path "/goodbye" >=> OK "Good bye GET" 
        ]
      POST >=> choose
        [ 
          path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" 
        ] 
    ]


startWebServer defaultConfig app
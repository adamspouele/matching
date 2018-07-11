#r "../../Suave.2.4.3/lib/net461/Suave.dll"


open Suave.State.CookieStateStore
open Suave 
open Suave.Json
open System.Runtime.Serialization
open Suave.Filters
open Suave.Operators
open Suave.Successful

//

let app =
  choose
    [ 
      path "/websocket" >=> handShake ws
      GET >=> choose
        [ 
          path "/hello" >=> OK "Hello GET"
          path "/goodbye" >=> OK "Good bye GET" 
          RequestErrors.NOT_FOUND "404 not Found."
        ]
      POST >=> choose
        [ 
          path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" 
          RequestErrors.NOT_FOUND "404 not Found."
        ]
    ]


startWebServer defaultConfig app
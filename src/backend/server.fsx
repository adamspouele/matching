#r "vendor/Suave.2.4.3/lib/net461/Suave.dll"
#r "vendor/Newtonsoft.Json.11.0.2/lib/net45/Newtonsoft.Json.dll"

open Suave 
open Suave.Json
open System.Runtime.Serialization
open Suave.Filters
open Suave.Operators
open Suave.Successful

module Controller = 
    let indexController =
        "test"
module Arithmetic = 
  let add x y =
      x + y
  let sub x y =
      x - y

let app =
  choose
    [ 
      //path "/websocket" >=> handShake ws
      GET >=> choose
        [ 
          path "/players" >=> OK Controller.indexController
          path "/players" >=> OK "Good bye GET" 
          pathScan "/add/%d/%d" (fun (a, b) -> OK((a + b).ToString()))
          RequestErrors.NOT_FOUND "404 not Found."
        ]
      POST >=> choose
        [ 
          path "/player/new" >=> OK "Hello POST"
          path "/players" >=> OK "Good bye POST" 
          RequestErrors.NOT_FOUND "404 not Found."
        ]
    ]


startWebServer defaultConfig app
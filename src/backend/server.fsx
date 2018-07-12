#r "vendor/Suave.2.4.3/lib/net461/Suave.dll"
#load "database.fsx"
#load "api.fsx"

open Suave
open Suave.Filters
open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket
open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Web

let playerWebPart = rest "player" {
  GetAll = Db.getPlayers
  Create = Db.createPlayer
  Update = Db.updatePlayer
  Delete = Db.deletePlayer
  GetById = Db.getPlayer
  UpdateById = Db.updatePlayerById
  IsExists = Db.isPersonExists
}


let ws (webSocket : WebSocket) (context: HttpContext) =
    socket {
       let mutable loop = true

       while loop do
            let! msg = webSocket.read()

            match msg with
            | (Text, data, true) ->
                let str = UTF8.toString data
                let response = sprintf "response to %s" str
                let byteResponse =
                    response
                    |> System.Text.Encoding.ASCII.GetBytes
                    |> ByteSegment
                do! webSocket.send Text byteResponse true

            | (Close, _, _) ->
                let emptyResponse = [||] |> ByteSegment
                do! webSocket.send Close emptyResponse true
                loop <- false

            | _ -> ()
    }


startWebServer defaultConfig (
  choose [
    path "/websocket" >=> handShake ws
    playerWebPart
  ]
)
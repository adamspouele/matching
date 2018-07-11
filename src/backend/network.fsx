

#r "../../Suave.2.4.3/lib/net461/Suave.dll"

namespace Matching

module Network = 
  open Suave 
  open Suave.Sockets
  open Suave.Sockets.Control
  open Suave.WebSocket

  let websocket (webSocket : WebSocket) (context: HttpContext) =
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
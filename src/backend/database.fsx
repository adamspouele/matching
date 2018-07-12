namespace SuaveRestApi.Db
open System.Collections.Generic

type Player = {
  Id : int
  Name : string
}
module Db =
  let private storage = new Dictionary<int, Player>()
  let getPlayers () =
    storage.Values |> Seq.map (fun p -> p)
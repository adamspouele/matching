namespace SuaveRestApi.Db
open System.Collections.Generic

type Player = {
  Id : int
  Name : string
}
module Db =
  let private playerStore = new Dictionary<int, Player>()
  let getPlayers () =
    playerStore.Values |> Seq.map (fun p -> p)
  let createPLayer player =
    let id = playerStore.Values.Count + 1
    let newPlayer = {
        Id = id
        Name = player.Name
    }
    playerStore.Add(id, newPlayer)
    newPlayer
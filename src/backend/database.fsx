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
  let createPlayer player =
    let id = playerStore.Values.Count + 1
    let newPlayer = {
        Id = id
        Name = player.Name
    }
    playerStore.Add(id, newPlayer)
    newPlayer
   
  let updatePlayerById palyerId playerToBeUpdated =
    if playerStore.ContainsKey(palyerId) then
      let updatedPlayer = {
        Id = palyerId
        Name = playerToBeUpdated.Name
      }
      playerStore.[palyerId] <- updatedPlayer
      Some updatedPlayer
    else
      None
  let updatePlayer playerToBeUpdated =
    updatePlayerById playerToBeUpdated.Id playerToBeUpdated

  let deletePlayer playerId =
    playerStore.Remove(playerId) |> ignore

  let getPlayer id =
      if playerStore.ContainsKey(id) then
        Some playerStore.[id]
      else
        None
  let isPersonExists = playerStore.ContainsKey
    

      
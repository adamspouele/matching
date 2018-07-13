namespace SuaveRestApi.Db
open System.Collections.Generic

type Player = {
  Id : int
  Name : string
  EarnedPairs : Dictionary<int, int>
}
module PlayerDb =
  let private playerStore = new Dictionary<int, Player>()
  let getPlayers () =
    playerStore.Values |> Seq.map (fun p -> p)
  let createPlayer player =
    let id = playerStore.Values.Count + 1
    let newEarnedPairs = new Dictionary<int, int>()
    let newPlayer = {
        Id = id
        Name = player.Name
        EarnedPairs = newEarnedPairs
    }
    playerStore.Add(id, newPlayer)
    newPlayer
   
  let updatePlayerById playerId playerToBeUpdated =
    if playerStore.ContainsKey(playerId) then
      let updatedPlayer = {
        Id = playerId
        Name = playerToBeUpdated.Name
        EarnedPairs = playerToBeUpdated.EarnedPairs
      }
      playerStore.[playerId] <- updatedPlayer
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
  let isPlayerExists = playerStore.ContainsKey

    

      
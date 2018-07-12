#r "vendor/Suave.2.4.3/lib/net461/Suave.dll"
#load "database.fsx"
#load "api.fsx"

open Suave
open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Web

let playerWebPart = rest "player" {
  GetAll = Db.getPlayers
  Create = Db.createPLayer
  Update = Db.updatePlayer
  Delete = Db.deletePlayer
  GetById = Db.getPlayer
  UpdateById = Db.updatePlayerById
  IsExists = Db.isPersonExists
}

startWebServer defaultConfig (
  choose [
    playerWebPart
  ]
)
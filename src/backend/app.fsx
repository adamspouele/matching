#r "vendor/Suave.2.4.3/lib/net461/Suave.dll"
#load "database.fsx"
#load "api.fsx"

open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Web

let playerWebPart = rest "player" {
  GetAll = Db.getPlayers
}

startWebServer defaultConfig playerWebPart
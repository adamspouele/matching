#r "vendor/Suave.2.4.3/lib/net461/Suave.dll"
#r "vendor/Newtonsoft.Json.11.0.2/lib/net45/Newtonsoft.Json.dll"

namespace SuaveRestApi.Rest
open Suave
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave.Operators
open Suave.Filters

open System.Text
open Suave.Utils.Compression


[<AutoOpen>]
module RestFul =
    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        Create : 'a -> 'a
    }

        // 'a -> WebPart
    let JSON v =
      let jsonSerializerSettings = new JsonSerializerSettings()
      jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

      JsonConvert.SerializeObject(v, jsonSerializerSettings)
      |> OK
      >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req : HttpRequest) =
      let getString_from_bytes = Encoding.UTF8.GetString : byte [] -> string
      let getString rawForm =
        getString_from_bytes(rawForm)
      req.rawForm |> getString |> fromJson<'a>    

    // string -> RestResource<'a> -> WebPart
    let rest resourceName resource =
      let resourcePath = "/" + resourceName
      let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
      path resourcePath >=> choose [
        GET >=> getAll
        POST >=> request (getResourceFromReq >> resource.Create >> JSON)
      ]

#r "vendor/Suave.2.4.3/lib/net461/Suave.dll"
#r "vendor/Newtonsoft.Json.11.0.2/lib/net45/Newtonsoft.Json.dll"

namespace SuaveRestApi.Rest
open Suave
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave.Operators
open Suave.Filters

[<AutoOpen>]
module RestFul =
    type RestResource<'a> = {
        GetAll : unit -> 'a seq
    }

        // 'a -> WebPart
    let JSON v =
      let jsonSerializerSettings = new JsonSerializerSettings()
      jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

      JsonConvert.SerializeObject(v, jsonSerializerSettings)
      |> OK
      >=> Writers.setMimeType "application/json; charset=utf-8"

    // string -> RestResource<'a> -> WebPart
    let rest resourceName resource =
      let resourcePath = "/" + resourceName
      let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
      path resourcePath >=> GET >=> getAll
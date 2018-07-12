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
        Update : 'a -> 'a option
        Delete : int -> unit
        GetById : int -> 'a option
        UpdateById : int -> 'a -> 'a option
        IsExists : int -> bool
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
      let resourceIdPath =
        new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
      let badRequest = Suave.RequestErrors.BAD_REQUEST "Resource not found"
      let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
      let handleResource requestError = function
        | Some r -> r |> JSON
        | _ -> requestError

      let deleteResourceById id =
        resource.Delete id
        NO_CONTENT    

      let getResourceById =
        resource.GetById >> handleResource (Suave.RequestErrors.NOT_FOUND "Resource not found")
      let updateResourceById id =
        request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

      choose [
        path resourcePath >=> choose [
          GET >=> getAll
          POST >=>
            request (getResourceFromReq >> resource.Create >> JSON)
          PUT >=>
            request (getResourceFromReq >> resource.Update >> handleResource badRequest)
        ]
        DELETE >=> pathScan resourceIdPath deleteResourceById
        GET >=> pathScan resourceIdPath getResourceById
        PUT >=> pathScan resourceIdPath updateResourceById
      ]

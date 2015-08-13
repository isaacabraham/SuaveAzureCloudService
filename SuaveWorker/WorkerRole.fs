namespace SuaveWorker

open Microsoft.WindowsAzure.ServiceRuntime
open Suave
open Suave.Http.Successful
open Suave.Types
open Suave.Web
open System.Net

type WorkerRole() = 
    inherit RoleEntryPoint()
       
    override __.Run() = 
        let config = 
            let endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.["SuaveEndpoint"].IPEndpoint
            { defaultConfig with bindings = 
                                     [ { HttpBinding.defaults with socketBinding = 
                                                                       { ip = endpoint.Address
                                                                         port = uint16 endpoint.Port } } ] }
        let app = OK <| sprintf "Hello from Suave. Running on Azure Cloud Service Instance '%s' with binding '%A'." RoleEnvironment.CurrentRoleInstance.Id config.bindings.Head
        startWebServer config app
    
    override __.OnStart() = 
        ServicePointManager.DefaultConnectionLimit <- 128
        base.OnStart()

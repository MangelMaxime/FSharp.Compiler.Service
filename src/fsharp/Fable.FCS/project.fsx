#if FABLE_COMPILER && !DOTNETCORE
// #r "System.Threading.dll"
#r "node_modules/fable-core/Fable.Core.dll"
#endif

#load
        "Fable.FCS.fsx"
        "app.fs"

[<EntryPoint>]
let main argv = App.main argv

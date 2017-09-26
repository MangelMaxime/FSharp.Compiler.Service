namespace Internal.Utilities

#nowarn "1182"

//------------------------------------------------------------------------
// shims for things not yet implemented in Fable
//------------------------------------------------------------------------

//TODO: implement proper Unicode char, decimal

module System =

    [<Struct>]
    type ValueTuple<'T1,'T2> =
        new (v1,v2) = { Item1 = v1; Item2 = v2 }
        val Item1 : 'T1
        val Item2 : 'T2

    module DateTime =
        let Now = System.DateTime.UtcNow //TODO: proper implementation

    // module Decimal =
    //     let GetBits(d: decimal): int[] = [| 0; 0; 0; 0 |] //TODO: proper implementation

    module Diagnostics =
        type Trace() =
            static member TraceInformation(s) = () //TODO: proper implementation

    module Reflection =
        type AssemblyName(assemblyName: string) =
            member x.Name = assemblyName //TODO: proper implementation

    module Threading =
        type CancellationToken(canceled: bool) =
            static member None = CancellationToken(false)
            member x.IsCancellationRequested = canceled

    type OperationCanceledException(ct: Threading.CancellationToken) =
        inherit System.Exception()

    type WeakReference<'T>(v: 'T) =
        member x.TryGetTarget () = (true, v)

    type StringComparer(comp: System.StringComparison) =
        static member Ordinal = StringComparer(System.StringComparison.Ordinal)
        static member OrdinalIgnoreCase = StringComparer(System.StringComparison.OrdinalIgnoreCase)
        interface System.Collections.Generic.IEqualityComparer<string> with
            member x.Equals(a,b) = System.String.Compare(a, b, comp) = 0
            member x.GetHashCode(a) =
                match comp with
                | System.StringComparison.Ordinal -> hash a
                | System.StringComparison.OrdinalIgnoreCase -> hash (a.ToLowerInvariant())
                | _ -> failwithf "Unsupported StringComparison: %A" comp
        interface System.Collections.Generic.IComparer<string> with
            member x.Compare(a,b) = System.String.Compare(a, b, comp)

    module Collections =
        module Concurrent =
            open System.Collections.Generic

            type ConcurrentDictionary<'TKey, 'TValue when 'TKey: equality> =
                inherit Dictionary<'TKey, 'TValue>
                new () = { inherit Dictionary<'TKey, 'TValue>() }
                new (comparer: IEqualityComparer<'TKey>) = { inherit Dictionary<'TKey, 'TValue>(comparer) }
                member x.TryAdd (key:'TKey, value:'TValue) = x.[key] <- value; true
                member x.GetOrAdd (key, valueFactory) =
                    match x.TryGetValue key with
                    | true, v -> v
                    | false, _ -> let v = valueFactory(key) in x.[key] <- v; v

    module IO =
        module Directory =
            let GetCurrentDirectory () = "." //TODO: proper xplat implementation

        module Path =
            // open System.Text.RegularExpressions

            let Combine (path1: string, path2: string) = //TODO: proper xplat implementation
                let path1 =
                    if (String.length path1) = 0 then path1
                    else (path1.TrimEnd [|'\\';'/'|]) + "/"
                path1 + (path2.TrimStart [|'\\';'/'|])

            let ChangeExtension (path: string, ext: string) =
                // Regex.Replace(path, "\.\w+$", ext)
                let i = path.LastIndexOf(".")
                if i < 0 then path
                else path.Substring(0, i) + ext

            let HasExtension (path: string) =
                // Regex.Match(path, "\.\w+$").Success
                let i = path.LastIndexOf(".")
                i >= 0

            let GetExtension (path: string) =
                // Regex.Match(path, "\.\w+$").Value
                let i = path.LastIndexOf(".")
                if i < 0 then ""
                else path.Substring(i)

            let GetInvalidPathChars () = //TODO: proper xplat implementation
                Seq.toArray "<>|\"\b\0\t"

            let GetInvalidFileNameChars () = //TODO: proper xplat implementation
                Seq.toArray "<>|\"\b\0\t"

            let GetFullPath (path: string) = //TODO: proper xplat implementation
                path

            let GetFileName (path: string) =
                let normPath = path.Replace("\\", "/").TrimEnd('/')
                let i = normPath.LastIndexOf("/")
                path.Substring(i + 1)

            let GetFileNameWithoutExtension (path: string) =
                let filename = GetFileName path
                // Regex.Replace(filename, "\.\w+$", "")
                let i = filename.LastIndexOf(".")
                if i < 0 then filename
                else filename.Substring(0, i)

            let GetDirectoryName (path: string) = //TODO: proper xplat implementation
                let normPath = path.Replace("\\", "/")
                let i = normPath.LastIndexOf("/")
                if i <= 0 then ""
                else path.Substring(0, i)

            let IsPathRooted (path: string) = //TODO: proper xplat implementation
                let normPath = path.Replace("\\", "/").TrimEnd('/')
                path.StartsWith("/")

    module Char =
        open System.Globalization

        let GetUnicodeCategory (c: char): UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
        let IsSurrogatePair (s,i) = false //TODO: proper Unicode implementation

        // let GetUnicodeCategory (c: char): UnicodeCategory = //TODO: proper Unicode implementation
        //     LanguagePrimitives.EnumOfValue (int categoryForLatin1.[int c])
        // let IsControl (c: char) =
        //     GetUnicodeCategory(c) = UnicodeCategory.Control
        // let IsDigit (c: char) =
        //     GetUnicodeCategory(c) = UnicodeCategory.DecimalDigitNumber
        // let IsLetter (c: char) =
        //     match GetUnicodeCategory(c) with
        //     | UnicodeCategory.UppercaseLetter
        //     | UnicodeCategory.LowercaseLetter
        //     | UnicodeCategory.TitlecaseLetter
        //     | UnicodeCategory.ModifierLetter
        //     | UnicodeCategory.OtherLetter -> true
        //     | _ -> false
        // let IsLetterOrDigit (c: char) =
        //     IsLetter(c) || IsDigit(c)
        // let IsWhiteSpace (c: char) =
        //     // There are characters which belong to UnicodeCategory.Control but are considered as white spaces.
        //     c = ' ' || (c >= '\x09' && c <= '\x0d') || c = '\xa0' || c = '\x85'
        // let IsUpper (c: char) =
        //     GetUnicodeCategory(c) = UnicodeCategory.UppercaseLetter
        // let IsLower (c: char) =
        //     GetUnicodeCategory(c) = UnicodeCategory.LowercaseLetter
        // let IsPunctuation (c: char) =
        //     match GetUnicodeCategory(c) with
        //     | UnicodeCategory.ConnectorPunctuation
        //     | UnicodeCategory.DashPunctuation
        //     | UnicodeCategory.OpenPunctuation
        //     | UnicodeCategory.ClosePunctuation
        //     | UnicodeCategory.InitialQuotePunctuation
        //     | UnicodeCategory.FinalQuotePunctuation
        //     | UnicodeCategory.OtherPunctuation -> true
        //     | _ -> false
        // let IsSurrogatePair (s,i) = false //TODO: proper Unicode implementation
        // let ToUpper (c: char) = if IsLower(c) then char(int('A') + (int(c) - int('a'))) else c
        // let ToLower (c: char) = if IsUpper(c) then char(int('a') + (int(c) - int('A'))) else c
        // let ToUpperInvariant (c: char) = ToUpper(c)
        // let ToLowerInvariant (c: char) = ToLower(c)

    module Text =

        type StringBuilder(?s: string) =
            let sb = System.Text.StringBuilder()
            do if Option.isSome s then sb.Append(s.Value) |> ignore
            new (capacity: int, ?maxCapacity: int) = StringBuilder()
            new (s: string, ?maxCapacity: int) = StringBuilder(s)
            member x.Append(s: string) = sb.Append(s) |> ignore; x
            member x.AppendFormat(fmt: string, o: obj) = sb.AppendFormat(fmt, o) |> ignore; x
            override x.ToString() = sb.ToString()

        // type StringBuilder(?s: string) =
        //     let buf = ResizeArray<string>()
        //     do if Option.isSome s then buf.Add(s.Value)
        //     new (capacity: int, ?maxCapacity: int) = StringBuilder()
        //     new (s: string, ?maxCapacity: int) = StringBuilder(s)
        //     member x.Append(s: string) = buf.Add(s); x
        //     member x.AppendFormat(fmt: string, o: obj) = buf.Add(System.String.Format(fmt, o)); x
        //     override x.ToString() = System.String.Concat(buf)

        module Encoding =

            module Unicode = // TODO: add surrogate pairs
                let GetBytes (s: string) =
                    let addUnicodeChar (buf: ResizeArray<byte>) (c: char) =
                        let i = int c
                        buf.Add (byte (i % 256))
                        buf.Add (byte (i / 256))
                    let buf = ResizeArray<byte>()
                    s.ToCharArray() |> Array.map (addUnicodeChar buf) |> ignore
                    buf.ToArray()

                let GetString (bytes: byte[], index: int, count: int) =
                    let sb = StringBuilder()
                    for i in 0 .. 2 .. count-1 do
                        let c = char ((int(bytes.[index+i+1]) <<< 8) ||| int(bytes.[index+i]))
                        sb.Append(string c) |> ignore
                    sb.ToString()

            module UTF8 = // TODO: add surrogate pairs

                let GetBytes (s: string) =
                    let buf = ResizeArray<byte>()
                    let encodeUtf8 (c: char) =
                        let i = int c
                        if i < 0x80 then
                            buf.Add (byte(i))
                        else if i < 0x800 then
                            buf.Add (byte(0xC0 ||| (i >>> 6 &&& 0x1F)))
                            buf.Add (byte(0x80 ||| (i &&& 0x3F)))
                        else if i < 0x10000 then
                            buf.Add (byte(0xE0 ||| (i >>> 12 &&& 0xF)))
                            buf.Add (byte(0x80 ||| (i >>> 6 &&& 0x3F)))
                            buf.Add (byte(0x80 ||| (i &&& 0x3F)))
                    s.ToCharArray() |> Array.map encodeUtf8 |> ignore
                    buf.ToArray()

                let decodeUtf8 (bytes: byte[]) (pos: byref<int>) =
                    let i1 = int(bytes.[pos])
                    if i1 &&& 0x80 = 0 then
                        pos <- pos + 1
                        (i1 &&& 0x7F)
                    else if i1 &&& 0xE0 = 0xC0 then
                        let i2 = int(bytes.[pos + 1]) in
                        pos <- pos + 2
                        ((i1 &&& 0x1F) <<< 6) ||| (i2 &&& 0x3F)
                    else if i1 &&& 0xF0 = 0xE0 then
                        let i2 = int(bytes.[pos + 1]) in
                        let i3 = int(bytes.[pos + 2]) in
                        pos <- pos + 3
                        ((i1 &&& 0x1F) <<< 12) ||| ((i2 &&& 0x3F) <<< 6) ||| (i3 &&& 0x3F)
                    else
                        pos <- pos + 1
                        0 // invalid decoding
                        
                let GetString (bytes: byte[], index: int, count: int) =
                    //let sb = StringBuilder()
                    let chars: char array = Array.zeroCreate count
                    let mutable pos = index
                    let mutable cnt = 0
                    let last = index + count
                    while pos < last do
                        let d = decodeUtf8 bytes &pos
                        // sb.Append(string (char d)) |> ignore
                        chars.[cnt] <- char d
                        // pos <- pos + inc
                        cnt <- cnt + 1
                    //sb.ToString()
                    System.String(chars, 0, cnt)


module Microsoft =
    module FSharp =
        module Control =
            type Async<'T> = FSharp.Control.Tasks.ContextSensitive.Async<'T>
            let async = FSharp.Control.Tasks.ContextSensitive.async
            module Async =
                let RunSynchronously = FSharp.Control.Tasks.Async.RunSynchronously
                let Sleep = FSharp.Control.Tasks.Async.Sleep

        module Collections =
            open System.Collections.Generic

            module HashIdentity =
                let inline FromFunctions hash eq : IEqualityComparer<'T> =
                    { new IEqualityComparer<'T> with
                        member __.GetHashCode(x) = hash x
                        member __.Equals(x,y) = eq x y  }
                let inline Structural<'T when 'T : equality> : IEqualityComparer<'T> =
                    FromFunctions LanguagePrimitives.GenericHash LanguagePrimitives.GenericEquality
                let Reference<'T when 'T : not struct > : IEqualityComparer<'T> = //TODO: proper implementation
                    FromFunctions LanguagePrimitives.GenericHash LanguagePrimitives.PhysicalEquality
                    // FromFunctions LanguagePrimitives.PhysicalHash LanguagePrimitives.PhysicalEquality

            module ComparisonIdentity = 
                let inline FromFunction comparer = 
                    { new IComparer<'T> with
                        member __.Compare(x,y) = comparer x y }
                let inline Structural<'T when 'T : comparison> : IComparer<'T> =
                    FromFunction LanguagePrimitives.GenericComparison

            module List =
                let indexed source = List.mapi (fun i x -> i,x) source

        module Core =
            module LanguagePrimitives =
                let FastGenericComparer<'T when 'T : comparison> =
                    Collections.ComparisonIdentity.Structural<'T>
                let PhysicalHash =
                    LanguagePrimitives.GenericHash //TODO: proper implementation

            module Operators =
                let (|Failure|_|) (exn: exn) = Some exn.Message
                    //if exn.GetType().FullName.EndsWith("Exception") then Some exn.Message else None
                let Failure message = new System.Exception(message)
                let nullArg x = raise(System.ArgumentNullException(x))

            module Printf =
                let bprintf (sb: System.Text.StringBuilder) =
                    let f (s:string) = sb.Append(s) |> ignore
                    Printf.kprintf f
                let fprintf (os: System.IO.TextWriter) =
                    let f (s:string) = System.Console.Write(s) //os.Write(s)
                    Printf.kprintf f

            //------------------------------------------------------------------------
            // From reshapedreflection.fs
            //------------------------------------------------------------------------
            module XmlAdapters =
                let s_escapeChars = [| '<'; '>'; '\"'; '\''; '&' |]
                let getEscapeSequence c =
                    match c with
                    | '<'  -> "&lt;"
                    | '>'  -> "&gt;"
                    | '\"' -> "&quot;"
                    | '\'' -> "&apos;"
                    | '&'  -> "&amp;"
                    | _ as ch -> ch.ToString()
                let escape str = String.collect getEscapeSequence str

        //------------------------------------------------------------------------
        // From sr.fs
        //------------------------------------------------------------------------
        module Compiler =
            module SR =
                let GetString(name:string) =
                    let ok, value = SR.Resources.resources.TryGetValue(name)
                    if ok then value
                    else "Missing FSStrings error message for: " + name

            module internal DiagnosticMessage =
                type ResourceString<'T>(sfmt: string, fmt: string) =
                    member x.Format =
                        let ar = fmt.Split('%')
                                |> Array.filter (fun s -> String.length s > 0)
                                |> Array.map (fun s -> box("%"+s))
                        let tmp = System.String.Format(sfmt, ar)
                        let fmt = Printf.StringFormat<'T>(tmp)
                        sprintf fmt

                let postProcessString (s : string) =
                    s.Replace("\\n","\n").Replace("\\t","\t")
                let DeclareResourceString ((messageID: string),(fmt: string)) =
                    let messageString = SR.GetString(messageID) |> postProcessString
                    ResourceString<'T>(messageString, fmt)

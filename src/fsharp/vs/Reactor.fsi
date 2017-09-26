// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

#if FABLE_COMPILER
open Internal.Utilities
open Microsoft.FSharp.Control
#endif
open System.Threading
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

/// Represents the capability to schedule work in the compiler service operations queue for the compilation thread
type internal IReactorOperations = 

    /// Put the operation in the queue, and return an async handle to its result. 
    abstract EnqueueAndAwaitOpAsync : userOpName:string * opName:string * opArg:string * action: (CompilationThreadToken -> Cancellable<'T>) -> Async<'T>

    /// Enqueue an operation and return immediately. 
    abstract EnqueueOp: userOpName:string * opName:string * opArg:string * action: (CompilationThreadToken -> unit) -> unit

#if !FABLE_COMPILER

/// Reactor is intended for long-running but interruptible operations, interleaved
/// with one-off asynchronous operations. 
///
/// It is used to guard the global compiler state while maintaining  responsiveness on 
/// the UI thread.
/// Reactor operations
[<Sealed>]
type internal Reactor =

    /// Set the background building function, which is called repeatedly
    /// until it returns 'false'.  If None then no background operation is used.
    member SetBackgroundOp : ( (* userOpName:*) string * (* opName: *) string * (* opArg: *) string *  (CompilationThreadToken -> CancellationToken -> bool)) option -> unit

    /// Cancel any work being don by the background building function.
    member CancelBackgroundOp : unit -> unit

    /// Block until the current implicit background build is complete. Unit test only.
    member WaitForBackgroundOpCompletion : unit -> unit

    /// Block until all operations in the queue are complete
    member CompleteAllQueuedOps : unit -> unit

    /// Enqueue an uncancellable operation and return immediately. 
    member EnqueueOp : userOpName:string * opName: string * opArg: string * op:(CompilationThreadToken -> unit) -> unit

    /// For debug purposes
    member CurrentQueueLength : int

    /// Put the operation in the queue, and return an async handle to its result. 
    member EnqueueAndAwaitOpAsync : userOpName:string * opName:string * opArg:string * (CompilationThreadToken -> Cancellable<'T>) -> Async<'T>

    /// The timespan in milliseconds before background work begins after the operations queue is empty
    member PauseBeforeBackgroundWork : int with get, set

    /// Get the reactor 
    static member Singleton : Reactor
  
#endif //!FABLE_COMPILER

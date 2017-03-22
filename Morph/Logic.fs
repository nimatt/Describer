namespace Morph

open System
open System.Linq.Expressions
open System.Dynamic
open System.Collections.Generic
open Newtonsoft.Json

type Converter<'b> (exps: seq<Expression<Func<'b,Object>>>) =
    let getNameFromSet (names: Set<option<string>>) =
        match Set.count names with
        | 0 -> None
        | _ -> names |> Set.toSeq |> Seq.map (fun n -> n.Value) |> (fun vs -> Some (String.Join(", ", vs)))    

    let rec getPropertyName (exp: Expression) = getPropertyNameInternal None exp
    and getPropertyNameInternal lastFound (exp: Expression) = 
        match exp with
            | :? MemberExpression as memExp -> getPropertyNameInternal (Some memExp.Member.Name) memExp.Expression
            | :? MethodCallExpression as methExp -> getNameFromMethodCall methExp
            | :? NewArrayExpression as newArrExp -> getNameFromMultiple newArrExp.Expressions
            | :? BinaryExpression as binExp -> getNameFromMultiple ([binExp.Left; binExp.Right])
            | :? ParameterExpression -> lastFound
            | :? ConstantExpression -> None
            | :? UnaryExpression as ue -> getPropertyNameInternal lastFound ue.Operand
            | null -> None
            | _ -> None

    and poolNames (names: seq<option<string>>) =
        names
            |> Seq.where (fun res -> res.IsSome)
            |> Set.ofSeq
            |> getNameFromSet
    and getNameFromMultiple (exps: seq<Expression>) =
        exps 
            |> Seq.map getPropertyName
            |> poolNames
    and getNameFromMethodCall (methExp: MethodCallExpression) = 
        let directPath = getPropertyNameInternal (Some (methExp.Method.Name + "()")) methExp.Object
        [| directPath; (methExp.Arguments |> getNameFromMultiple) |]
                |> poolNames
        

    let compiled =
        let namedExps = exps |> Seq.map (fun e -> (getPropertyName (e.Body), e)) |> List.ofSeq
        if Seq.exists (fun ((n: option<string>), e) -> n.IsNone) namedExps then
            raise (new ArgumentException("Unable to get name from expression"))
        else
            namedExps |> Seq.map (fun (n,e) -> (n.Value, e.Compile())) |> List.ofSeq

    member this.Evaluate (o: 'b) = 
        let obj = new ExpandoObject()
        compiled 
            |> Seq.iter (fun ce -> 
                (obj :> IDictionary<string, Object>).Add(fst ce, (snd ce).Invoke(o)))
        obj

module Morpher = 
    let Morph<'T> (o:'T) ([<ParamArray>] exps: Expression<Func<'T,Object>>[]): Object =
        let builder = Converter exps
        upcast builder.Evaluate o

    let Describe<'T> (o:'T) ([<ParamArray>] exps: Expression<Func<'T,Object>>[]): string = 
        JsonConvert.SerializeObject(Morph o exps)


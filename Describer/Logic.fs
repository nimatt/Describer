namespace Describer

open System
open System.Linq.Expressions

type private Builder<'b> (exps: seq<Expression<Func<'b,string>>>) =
    let getNameFromSet (names: Set<option<string>>) =
        match Set.count names with
        | 0 -> None
        | 1 -> names |> Set.toSeq |> Seq.head
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
            | :? UnaryExpression -> None
            | null -> None
            | _ -> raise (new Exception("Unknown expression type"))

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
        exps |> Seq.map (fun e -> ((getPropertyName (e.Body)).Value, e.Compile()))

    member this.Evaluate (o: 'b) = 
        compiled 
            |> Seq.map (fun ce -> fst ce + ": " + (snd ce).Invoke(o))
            |> (fun rw -> "{ " + String.Join("; ", rw) + " }")

module DescriptionBuilder = 
    let Describe<'T> (o:'T) ([<ParamArray>] exps: Expression<Func<'T,string>>[]): string = 
        let builder = Builder exps
        builder.Evaluate o


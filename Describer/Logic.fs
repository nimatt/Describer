namespace Describer

open System
open System.Linq.Expressions

type private Builder<'b> (exps: seq<Expression<Func<'b,string>>>) =

    let rec getPropertyName (exp: Expression) = getPropertyNameInternal None exp
    and getPropertyNameInternal lastFound (exp: Expression) = 
        match exp with
            | :? MemberExpression as memExp -> getPropertyNameInternal (Some memExp.Member.Name) memExp.Expression
            | :? MethodCallExpression as methExp -> getNameFromMethodCall methExp
            | :? ParameterExpression -> lastFound
            | :? ConstantExpression -> None
            | _ -> Some (exp.GetType().Name)

    and getNameFromMethodCall (methExp: MethodCallExpression) = 
        let directPath = getPropertyNameInternal (Some (methExp.Method.Name + "()")) methExp.Object
        directPath :: (methExp.Arguments |> Seq.map (fun arg -> getPropertyName arg) |> List.ofSeq)
             |> Seq.where (fun res -> res.IsSome) 
             |> Seq.head

    let compiled =
        exps |> Seq.map (fun e -> ((getPropertyName (e.Body)).Value, e.Compile()))

    member this.Evaluate (o: 'b) = 
        compiled |> Seq.map (fun ce -> fst ce + ": " + (snd ce).Invoke(o)) |> (fun rw -> String.Join(", ", rw))

module DescriptionBuilder = 
    let Describe<'T> (o:'T) ([<ParamArray>] exps: Expression<Func<'T,string>>[]): string = 
        let builder = Builder exps
        builder.Evaluate o


namespace Describer

open System
open System.Linq.Expressions

type private Builder<'b> (exps: seq<Expression<Func<'b,string>>>) =
    let isLastExpr (exp: Expression) = exp.NodeType = ExpressionType.Parameter

    let rec getPropertyName (exp: Expression) = 
        match exp with
            | :? MemberExpression as memExp -> if isLastExpr (memExp.Expression) then memExp.Member.Name
                                               else getPropertyName (memExp.Expression)
            | :? MethodCallExpression as methExp -> if isLastExpr (methExp.Object) then methExp.Method.Name + "()"
                                                    else getPropertyName (methExp.Object)
            | :? ParameterExpression as paramExp -> paramExp.Name
            | _ -> exp.GetType().Name

    let compiled =
        exps |> Seq.map (fun e -> (getPropertyName (e.Body), e.Compile()))

    member this.Evaluate (o: 'b) = 
        compiled |> Seq.map (fun ce -> fst ce + ": " + (snd ce).Invoke(o)) |> (fun rw -> String.Join(", ", rw))

module DescriptionBuilder = 
    let Describe<'T> (o:'T) ([<ParamArray>] exps: Expression<Func<'T,string>>[]): string = 
        let builder = Builder exps
        builder.Evaluate o


namespace Lotus.Semantics.Binding;

internal partial class Binder
{
    private bool TryBindAndConvertArgs(
        FunctionInfo func,
        ImmutableArray<Expression> args,
        out ImmutableArray<Expression> convertedArgs
    ) {
        convertedArgs = [];

        // if there's more args than parameters, then there's definitely no match
        if (func.Parameters.Count < args.Length)
            return false;

        if (func.Parameters.Count > args.Length) {
            throw new NotImplementedException("todo: check for params w/ default values");
        }

        var convertedBuilder = ImmutableArray.CreateBuilder<Expression>(args.Length);

        foreach (var (arg, param) in args.Zip(func.Parameters)) {
            var actual = arg.Type;
            var expected = param.Type;

            if (actual.TryConvertTo(expected, out var conv)) {
                // if they're the same type, then just add the expression without casting
                if (conv is IdentityConversion) {
                    convertedBuilder.Add(arg);
                    continue;
                }

                throw new NotImplementedException($"don't know how to create Cast from Conversion of type {conv.GetType()}");
            }

            return false;
        }

        return true;
    }
}
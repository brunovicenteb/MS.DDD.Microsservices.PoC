namespace Toolkit;

public static class EnvinromentReader
{
    public static T Read<T>(string varName, T defaultValue = default, string varEmptyError = null)
        where T : IComparable, IConvertible, IComparable<T>, IEquatable<T>
    {
        bool hasDefault = !EqualityComparer<T>.Default.Equals(defaultValue, default);
        if (varName.IsEmpty())
            if (hasDefault)
                return defaultValue;
            else
                throw new ArgumentNullException(nameof(varName));
        var value = Environment.GetEnvironmentVariable(varName);
        if (value.IsEmpty())
            if (hasDefault)
                return defaultValue;
            else
                throw new NullReferenceException(varEmptyError);
        T result = (T)Convert.ChangeType(value, typeof(T));
        if (!EqualityComparer<T>.Default.Equals(result, default))
            return result;
        return defaultValue;
    }
}
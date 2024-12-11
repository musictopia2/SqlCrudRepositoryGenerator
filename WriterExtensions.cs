namespace SqlCrudRepositoryGenerator;
internal static class WriterExtensions
{
    private static IWriter AddIServiceCollection(this IWriter w)
    {
        w.Write("global::Microsoft.Extensions.DependencyInjection.IServiceCollection");
        return w;
    }
    public static IWriter AddActionParameter(this IWriter w, bool nullable)
    {
        w.Write("global::System.Action<")
            .AddIServiceCollection()
            .Write(">");
        if (nullable)
        {
            w.Write("?");
        }
        w.Write("actions");
        return w;
    }
    public static IWriter AddStartExtensionMethod(this IWriter w, Action<IWriter> action)
    {
        w.Write("public static ")
            .AddIServiceCollection()
            .Write(" ");
        action.Invoke(w);
        w.Write("(this ")
            .AddIServiceCollection()
            .Write(" services");
        return w;
    }
    public static IWriter AddConnectionString(this IWriter w, bool needsComma = true)
    {
        w.Write(", string connectionString");
        if (needsComma)
        {
            w.Write(", ");
        }
        return w;
    }
}
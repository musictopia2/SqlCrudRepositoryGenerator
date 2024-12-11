namespace SqlCrudRepositoryGenerator;
internal static class SourceBuilderExtensions
{
    public static void WriteServiceExtensionsStub(this SourceCodeStringBuilder builder, Action<ICodeBlock> action, ICustomResult result, bool useGlobalUsings)
    {
        builder.WriteLine("#nullable enable");
        if (useGlobalUsings)
        {
            builder.WriteLine($"global using {result.Namespace}.{result.ClassName};");
        }
        builder.WriteLine($"namespace {result.Namespace}.{result.ClassName};")
            .WriteLine($"public static class {result.ClassName}")
            .WriteCodeBlock(action.Invoke);

    }
}
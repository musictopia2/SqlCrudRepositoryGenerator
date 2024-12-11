namespace SqlCrudRepositoryGenerator;
internal class EntityParserClass(IEnumerable<ClassDeclarationSyntax> list, Compilation compilation)
{
    public BasicList<EntityAloneModel> GetResults()
    {
        BasicList<EntityAloneModel> output = [];
        foreach (var item in list)
        {
            EntityAloneModel results = GetResult(item);
            if (output.Any(x => x.ClassName == results.ClassName) == false)
            {
                output.Add(results);
            }
        }
        return output;
    }
    private EntityAloneModel GetResult(ClassDeclarationSyntax classDeclaration)
    {
        EntityAloneModel output;
        INamedTypeSymbol symbol = compilation.GetClassSymbol(classDeclaration)!;
        output = symbol.GetStartingResults<EntityAloneModel>();
        output.SourceGeneratedNamespace = $"{compilation.AssemblyName}.SourceCrudGeneratorClasses";
        return output;
    }
}
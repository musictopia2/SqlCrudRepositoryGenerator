namespace SqlCrudRepositoryGenerator;
internal class RepositoryParserClass(IEnumerable<ClassDeclarationSyntax> list, Compilation compilation)
{
    public BasicList<RepositoryAloneModel> GetResults()
    {
        BasicList<RepositoryAloneModel> output = [];
        foreach (var item in list)
        {
            RepositoryAloneModel results = GetResult(item);
            if (output.Any(x => x.ClassName == results.ClassName) == false)
            {
                output.Add(results);
            }
        }
        return output;
    }
    private RepositoryAloneModel GetResult(ClassDeclarationSyntax classDeclaration)
    {
        RepositoryAloneModel output;
        INamedTypeSymbol symbol = compilation.GetClassSymbol(classDeclaration)!;
        output = symbol.GetStartingResults<RepositoryAloneModel>();
        INamedTypeSymbol other = (INamedTypeSymbol)symbol.BaseType!.TypeArguments.Single();
        var fins = other.GetStartingResults<RepositoryAloneModel>()!;
        output.EntityNamespace = fins.Namespace;
        output.EntityClassName = fins.ClassName;
        return output;
    }
}
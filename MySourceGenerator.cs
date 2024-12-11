namespace SqlCrudRepositoryGenerator;
[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Create syntax provider for entities
        IncrementalValuesProvider<ClassDeclarationSyntax> entityProvider1 = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsEntityTarget(s),
            (t, _) => GetEntityTarget(t)
        ).Where(m => m != null)!;

        // Step 2: Create syntax provider for repositories
        IncrementalValuesProvider<ClassDeclarationSyntax> repositoryProvider1 = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsRepositoryTarget(s),
            (t, _) => GetRepositoryTarget(t)
        ).Where(m => m != null)!;


        var entityCompile = context.CompilationProvider.Combine(entityProvider1.Collect());

        var entityProvider2 = entityCompile.SelectMany(static (x, _) =>
        {
            ImmutableHashSet<ClassDeclarationSyntax> start = [.. x.Right];
            return GetEntityResults(start, x.Left);
        });

        var repositoryCompile = context.CompilationProvider.Combine(repositoryProvider1.Collect());

        var repositoryProvider2 = repositoryCompile.SelectMany(static (x, _) =>
        {
            ImmutableHashSet<ClassDeclarationSyntax> start = [.. x.Right];
            return GetRepositoryResults(start, x.Left);
        });



        // Step 3: Collect results for entities and repositories
        var entityResults = entityProvider2.Collect();
        var repositoryResults = repositoryProvider2.Collect();

        // Step 4: Combine both providers manually
        var combinedResults = entityResults.Combine(repositoryResults).Select((combined, _) =>
        {
            ImmutableArray<EntityAloneModel> entities = combined.Left;
            ImmutableArray<RepositoryAloneModel> repositories = combined.Right;
            return GetCompleteResults(entities, repositories);
        });
        //var combinedResults2 = combinedResults1.Collect();

        // Step 5: Register the combined results for code generation
        context.RegisterSourceOutput(combinedResults, Execute);
    }

    // Check if the class is a target entity
    private bool IsEntityTarget(SyntaxNode syntax)
    {
        return syntax is ClassDeclarationSyntax ctx &&
            ctx.IsPublic();
    }
    // Get the entity model (e.g., EntityAloneModel)
    private ClassDeclarationSyntax? GetEntityTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode();
        var symbol = context.GetClassSymbol(ourClass);
        bool rets = symbol.Implements("ISimpleDatabaseEntity");
        if (rets)
        {
            return ourClass;
        }
        return null;
    }
    private static ImmutableHashSet<EntityAloneModel> GetEntityResults(
        ImmutableHashSet<ClassDeclarationSyntax> classes,
        Compilation compilation
        )
    {
        EntityParserClass parses = new(classes, compilation);
        BasicList<EntityAloneModel> output = parses.GetResults();
        return [.. output];
    }

    // Check if the class is a target repository
    private bool IsRepositoryTarget(SyntaxNode syntax)
    {
        return syntax is ClassDeclarationSyntax ctx &&
               ctx.BaseList is not null &&
               ctx.ToString().Contains("SQLRepository");
    }

    // Get the repository model (e.g., RepositoryAloneModel)
    private ClassDeclarationSyntax? GetRepositoryTarget(GeneratorSyntaxContext context)
    {
        return context.GetClassNode();
    }

    private static ImmutableHashSet<RepositoryAloneModel> GetRepositoryResults(
       ImmutableHashSet<ClassDeclarationSyntax> classes,
       Compilation compilation)
    {
        RepositoryParserClass parses = new(classes, compilation);
        BasicList<RepositoryAloneModel> output = parses.GetResults();
        return [.. output];
    }


    // Combine entities and repositories into a unified list of CombinedModel
    private static ResultsModel GetCompleteResults(
        ImmutableArray<EntityAloneModel> entities,
        ImmutableArray<RepositoryAloneModel> repositories)
    {
        ResultsModel output = new();
        output.ClassName = "SourceGeneratedCrudServiceExtensions";
        string mainNamespace = "";
        BasicList<CombinedModel> fins = [];
        foreach (EntityAloneModel entity in entities)
        {
            CombinedModel combine = new();
            mainNamespace = entity.SourceGeneratedNamespace;
            combine.Entity = $"{entity.Namespace}.{entity.ClassName}";
            RepositoryAloneModel? repository = repositories.SingleOrDefault(x => x.EntityClassName == entity.ClassName);
            if (repository is not null)
            {
                combine.Repository = $"{repository.Namespace}.{repository.ClassName}";
            }
            fins.Add(combine);

        }
        output.Entities = [.. fins];
        output.Namespace = mainNamespace;
        return output;
    }

    // Emit the combined models
    private void Execute(SourceProductionContext context, ResultsModel combinedList)
    {
        // Example: Emit the combined source code
        EmitClass emitter = new(combinedList, context);
        emitter.Emit();
    }
}
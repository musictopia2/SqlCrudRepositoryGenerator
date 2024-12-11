namespace SqlCrudRepositoryGenerator;
internal static class CodeBlockExtensions
{
    public static ICodeBlock AddEntityConstraint(this ICodeBlock w)
    {
        w.WriteLine("where TEntity : class, global::CommonBasicLibraries.DatabaseHelpers.EntityInterfaces.ISimpleDatabaseEntity, ICommandQuery<TEntity>, ITableMapper<TEntity>");
        return w;
    }
    public static ICodeBlock AddISqlDatabaseConfigurationConstraint(this ICodeBlock w)
    {
        w.WriteLine("where D : global::CommonBasicLibraries.DatabaseHelpers.AbstractStaticInterfaces.ISqlDatabaseConfiguration");
        return w;
    }
    public static ICodeBlock AddActionInvoke(this ICodeBlock w, bool nullable)
    {
        if (nullable)
        {
            w.WriteLine("actions?.Invoke(services);");
        }
        else
        {
            w.WriteLine("actions.Invoke(services);");
        }
        return w;
    }
    public static ICodeBlock AddReturnServices(this ICodeBlock w)
    {
        w.WriteLine("return services;");
        return w;
    }
    public static ICodeBlock AddRegisterCustomRepositories(this ICodeBlock w, ResultsModel result)
    {
        var list = result.Entities.Where(x => x.Repository != "");
        foreach (var entity in list)
        {
            w.WriteLine($"services.RegisterCustomRepositories<{entity.Entity}>(connectionString);");
        }
        return w;
    }
    public static ICodeBlock AddRegisterDatabaseRepositories(this ICodeBlock w, ResultsModel result)
    {
        var list = result.Entities.Where(x => x.Repository == "");
        w.WriteLine("global::AdoNetHelpersLibrary.RepositoryHelpers.SqlExtensions.AddDatabaseRepositories(services, connectionString, opts =>")
            .WriteLambaBlock(w =>
            {
                foreach (var item in list)
                {
                    w.WriteLine($"opts.Add<{item.Entity}>();");
                }
            });
        return w;
    }
    public static ICodeBlock AddFinishServices(this ICodeBlock w, ResultsModel result)
    {
        var list = result.Entities.Where(x => x.Repository != "");
        foreach (var entity in list)
        {
            w.WriteLine($"if (typeof(TEntity) == typeof({entity.Entity}))")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("services.AddScoped(sp =>")
                .WriteLambaBlock(w =>
                {
                    w.WriteLine("global::CommonBasicLibraries.CrudRepositoryHelpers.IDatabaseConnectionManager manager = sp.GetRequiredService<global::CommonBasicLibraries.CrudRepositoryHelpers.IDatabaseConnectionManager>();");
                    w.WriteLine($"{entity.Repository} repository = new(connectionString, manager);");
                    w.WriteLine("return repository;");
                })
                .WriteLine($"services.AddScoped<global::AdoNetHelpersLibrary.RepositoryHelpers.SQLRepository<{entity.Entity}>>(sp => sp.GetRequiredService<{entity.Repository}>());")
            .WriteLine($"services.AddScoped<global::CommonBasicLibraries.CrudRepositoryHelpers.IRepository<{entity.Entity}>>(sp => sp.GetRequiredService<{entity.Repository}>());")
            .WriteLine($"services.AddScoped<global::CommonBasicLibraries.CrudRepositoryHelpers.IRepositoryQueryAdvanced<{entity.Entity}>>(sp => sp.GetRequiredService<{entity.Repository}>());"); ;
            });
        }
        return w;
    }
}
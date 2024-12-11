namespace SqlCrudRepositoryGenerator;
internal class EmitClass(ResultsModel result, SourceProductionContext context)
{
    public void Emit()
    {
        WriteItem();
    }
    private void WriteItem()
    {
        SourceCodeStringBuilder builder = new();
        builder.WriteServiceExtensionsStub(PopulateDetails, result, true);
        context.AddSource($"CrudServiceExtensions.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateDetails(ICodeBlock w)
    {
        WriteRegisterAllCustomRepositories1(w);
        WriteRegisterAllCustomRepositories2(w);
        WriteRegisterAllRepositories1(w);
        WriteRegisterAllRepositories2(w);
        WriteRegisterCustomRepositories1(w);
        WriteRegisterCustomRepositories2(w);
    }
    private void WriteRegisterAllCustomRepositories1(ICodeBlock w)
    {
        w.WriteLine(w =>
        {
            w.AddStartExtensionMethod(w =>
            {
                w.Write("RegisterAllCustomRepositories<D>");
            })
            .Write(", ")
            .AddActionParameter(true)
            .Write(")");
        })
            .AddISqlDatabaseConfigurationConstraint()
            .WriteCodeBlock(w =>
            {
                w.WriteLine("return services.RegisterAllCustomRepositories(D.DatabaseName, actions);");
            });
    }

    private void WriteRegisterAllCustomRepositories2(ICodeBlock w)
    {
        w.WriteLine(w =>
        {
            w.AddStartExtensionMethod(w =>
            {
                w.Write("RegisterAllCustomRepositories");
            })
            .AddConnectionString()
            .AddActionParameter(true)
            .Write(")");
        }).WriteCodeBlock(w =>
        {
            w.AddActionInvoke(true);
            w.AddRegisterCustomRepositories(result);
            w.AddReturnServices();
        });
    }
    private void WriteRegisterAllRepositories1(ICodeBlock w)
    {
        w.WriteLine(w =>
        {
            w.AddStartExtensionMethod(w =>
            {
                w.Write("RegisterAllRepositories<D>");
            }).Write(", ")
            .AddActionParameter(false)
            .Write(")");
        }).AddISqlDatabaseConfigurationConstraint()
          .WriteCodeBlock(w =>
          {
              w.WriteLine("return services.RegisterAllRepositories(D.DatabaseName, actions);");
          });
    }
    private void WriteRegisterAllRepositories2(ICodeBlock w)
    {
        w.WriteLine(w =>
        {
            w.AddStartExtensionMethod(w =>
            {
                w.Write("RegisterAllRepositories");
            }).AddConnectionString()
            .AddActionParameter(false)
            .Write(")");

        }).WriteCodeBlock(w =>
        {
            w.AddActionInvoke(false);
            w.WriteLine("services.RegisterAllCustomRepositories(connectionString, null);")
            .AddRegisterDatabaseRepositories(result)
            .AddReturnServices();
        });
    }
    private void WriteRegisterCustomRepositories1(ICodeBlock w)
    {
        w.WriteLine(w =>
        {
            w.AddStartExtensionMethod(w =>
            {
                w.Write("RegisterCustomRepositories<TEntity, D>");
            })
            .Write(")");
        })
            .AddEntityConstraint()
            .AddISqlDatabaseConfigurationConstraint()
            .WriteCodeBlock(w =>
            {
                w.WriteLine("return services.RegisterCustomRepositories<TEntity>(D.DatabaseName);");
            });
    }
    private void WriteRegisterCustomRepositories2(ICodeBlock w)
    {
        w.WriteLine(w =>
        {
            w.AddStartExtensionMethod(w =>
            {
                w.Write("RegisterCustomRepositories<TEntity>");
            }).AddConnectionString(false)
            .Write(")");
        }).AddEntityConstraint()
        .WriteCodeBlock(w =>
        {
            w.AddFinishServices(result)
            .AddReturnServices();
        });
    }
}
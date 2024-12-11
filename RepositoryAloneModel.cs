namespace SqlCrudRepositoryGenerator;
internal record RepositoryAloneModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string EntityClassName { get; set; } = "";
    public string EntityNamespace { get; set; } = "";
}
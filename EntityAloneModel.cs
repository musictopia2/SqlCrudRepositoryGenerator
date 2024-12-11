namespace SqlCrudRepositoryGenerator;
internal record EntityAloneModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = ""; //i think this is all i need
    public string SourceGeneratedNamespace { get; set; } = ""; //this is where its going to go.
}
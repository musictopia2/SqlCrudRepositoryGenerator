namespace SqlCrudRepositoryGenerator;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public ImmutableArray<CombinedModel> Entities { get; set; } = [];
}
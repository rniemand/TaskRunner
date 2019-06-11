namespace TaskRunner.Shared.Providers
{
  public struct ProviderInput
  {
    public string Name { get; set; }
    public bool Required { get; set; }
    public string DefaultValue { get; set; }
  }
}

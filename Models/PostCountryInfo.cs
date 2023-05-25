namespace DotNetWebAPI.Models;

public partial class PostCountryInfo
{
    public int CountryId {get; set;}
    public int DatorId { get; set; }
    public string CountryName {get; set;}="";
    public bool IsDemocracy {get; set;}
    public decimal PopulationInMillion { get; set; }
    public DateTime DataCreated { get; set; }

    public DateTime DataUpdated { get; set; }

}
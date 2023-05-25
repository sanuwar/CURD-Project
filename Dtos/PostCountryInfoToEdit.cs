namespace DotNetWebAPI.Dtos;

public partial class PostCountryInfoToEdit
{
   public int CountryId {get; set;}
    public string CountryName {get; set;}="";
    public bool IsDemocracy {get; set;}
    public decimal PopulationInMillion { get; set; }

}
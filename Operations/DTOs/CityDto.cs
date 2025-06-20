namespace CityHotelGarageAPI.Operations.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Population { get; set; }
    public DateTime CreatedDate { get; set; }
    public int HotelCount { get; set; }
}

public class CityCreateDto
{
    public string Name { get; set; } = "";
    public int Population { get; set; }
}
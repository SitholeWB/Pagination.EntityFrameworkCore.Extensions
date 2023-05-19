namespace WebApi.DTOs
{
	public class CountryDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTimeOffset DateAdded { get; set; }
		public DateTimeOffset LastModifiedDate { get; set; }
	}
}
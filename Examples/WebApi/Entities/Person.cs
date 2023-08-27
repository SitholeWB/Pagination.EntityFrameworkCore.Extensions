namespace WebApi.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public Guid CountryId { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public virtual Country? Country { get; set; }
    }
}

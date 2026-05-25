namespace Domain.Entities.SystemModule.ModelsModule
{
    public class AIModel : BaseEntity<int>
    {
        public string Name { get; set; }
        public int SystemId { get; set; }
        public SystemRoot System { get; set; }
    }
}

using GotIt.Common.Enums;

namespace GotIt.BLL.ViewModels
{
    public class PersonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EAgeStage AgeStage { get; set; }
        public EGender Gender { get; set; }
        public string Embeddings { get; set; }
    }
}
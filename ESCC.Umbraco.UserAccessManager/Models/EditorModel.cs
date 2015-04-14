using PetaPoco;

namespace ESCC.Umbraco.UserAccessManager.Models
{
    [TableName("editors")]
    public class EditorModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; }
    }
}
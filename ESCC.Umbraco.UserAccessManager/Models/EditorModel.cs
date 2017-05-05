using PetaPoco;

namespace Escc.Umbraco.UserAccessManager.Models
{
    [TableName("editors")]
    public class EditorModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in activity logs
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[UserId:{0},FullName:{1}]", UserId, FullName);
        }
    }
}
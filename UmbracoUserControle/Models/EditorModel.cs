using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoUserControl.Models
{
    [PetaPoco.TableName("editors")]
    public class EditorModel
    {
        public int UserId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    public class CharacterHistoryModel
    {
        public List<Skill> Skills { get; set; }

        public CharacterHistoryModel()
        {
            Skills = new List<Skill>();
        }
    }
}
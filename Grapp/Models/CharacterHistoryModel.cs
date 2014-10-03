using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    public class CharacterHistoryModel
    {
        public List<Skill> Highscores { get; set; }

        public CharacterHistoryModel()
        {
            Highscores = new List<Skill>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    public class CharacterHistoryModel
    {
        public List<Skill> Highscores { get; set; }
        public List<Skill> SkillIncrease { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CurrentUpdate { get; set; }

        public string ErrorMessage { get; set; }

        public CharacterHistoryModel()
        {
            Highscores = new List<Skill>();
            SkillIncrease = new List<Skill>();
        }
    }
}
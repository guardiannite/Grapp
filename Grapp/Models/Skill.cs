using Grapp.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    public class Skill
    {
        public Skills SkillType { get; set; }
        public long Level { get; set; }
        public long HighscoreRank { get; set; }
        public long Experience { get; set; }

        public static Skill FromEntity(SkillEntity entity)
        {
            return new Skill
            {
                Level = entity.Level.HasValue ? entity.Level.Value : 0,
                HighscoreRank = entity.Rank.HasValue ? entity.Rank.Value : 0,
                Experience = entity.Experience.HasValue ? entity.Experience.Value : 0,
                SkillType = (Skills)entity.SkillEnum.OrderIndex
            };
        }
    }
}
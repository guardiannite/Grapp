using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    public class Skill
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }

        public static string[] SkillNames 
        { 
            get
            {
                return _skillNames;
            }
        }

        //This order is very sensitive
        //It is the order of how the highscores report a player's highscore
        //http://services.runescape.com/m=rswiki/en/Hiscores_APIs
        private static string[] _skillNames = new string[]{
            "Overall",
            "Attack",
            "Defence",
            "Strength",
            "Constitution",
            "Ranged",
            "Prayer",
            "Magic",
            "Cooking",
            "WoodCutting",
            "Fletching",
            "Fishing",
            "FireMaking",
            "Crafting",
            "Smithing",
            "Mining",
            "Herblore",
            "Agility",
            "Thieving",
            "Slayer",
            "Farming",
            "RuneCrafting",
            "Hunter",
            "Construction",
            "Summoning",
            "Dungeoneering",
            "Divination",
        };
    }
}
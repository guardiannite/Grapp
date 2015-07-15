using Grapp.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class HighscoreParser
    {
        public static List<Skill> Parse(string serializedString)
        {
            var retVal = new List<Skill>();
            int skillCount = -1;

            //Ask the database how many skills there are (could extract this to a different location)
            using (var context = new GrappContext())
            {
                skillCount = context.SkillEnums.Count();
            }

            //Each line represents a highscore skill or minigame
            var lines = serializedString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            //It's common that Runescape addes more minigames, so this just verifies we got data for at least the skills
            if (lines.Length < skillCount)
            {
                throw new ArgumentException(String.Format("Expecting to parse at least ({0}) lines of highscore string, but only received ({1})", skillCount, lines.Length));
            }

            //Highscore for each skill
            //The order of each skill is the same as the Skills enum
            for (int i = 0; i < skillCount; i++)
            {
                //Always recieve three numbers per row for skills
                var skillHighscoreInfo = lines[i].Split(',');
                if(skillHighscoreInfo.Length != 3)
                {
                    throw new ArgumentException(String.Format("Error parsing line ({0}) of highscore string.  Expecting 3 comma separated values, but received ({1})", i, skillHighscoreInfo.Length));
                }

                //Rank, Level, Experience
                long[] highscoreValues = new long[3];

                for(int j = 0; j < highscoreValues.Length; j++)
                {
                    if(!long.TryParse(skillHighscoreInfo[j], out highscoreValues[j]))
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Error parsing line ({0}) of highscore string.  Could not parse ({1}) to an integer", i, skillHighscoreInfo[j]));
                    }
                }

                retVal.Add(new Skill()
                {
                    HighscoreRank = highscoreValues[0],
                    Level = highscoreValues[1],
                    Experience = highscoreValues[2],
                    SkillType = (Skills)i,
                });
            }

            return retVal;
        }
    }
}

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
        public static List<Skill> GetSkills(string serializedString)
        {
            var retVal = new List<Skill>();
            var lines = serializedString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            if(lines.Length < Skill.SkillNames.Length)
            {
                throw new ArgumentException(String.Format("Expecting to parse at least ({0}) lines of highscore string, but only received ({1})", Skill.SkillNames.Length, lines.Length));
            }

            //Highscore for each skill
            for (int i = 0; i < Skill.SkillNames.Length; i++ )
            {
                var skillHighscoreInfo = lines[i].Split(',');
                if(skillHighscoreInfo.Length != 3)
                {
                    throw new ArgumentException(String.Format("Error parsing line ({0}) of highscore string.  Expecting 3 comma separated values, but received ({1})", i, skillHighscoreInfo.Length));
                }

                //Rank, Level, Experience
                int[] highscoreValues = new int[3];

                for(int j = 0; j < highscoreValues.Length; j++)
                {
                    if(!int.TryParse(skillHighscoreInfo[j], out highscoreValues[j]))
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Error parsing line ({0}) of highscore string.  Could not parse ({1}) to an integer", i, skillHighscoreInfo[j]));
                    }
                }

                retVal.Add(new Skill()
                {
                    Name = Skill.SkillNames[i],
                    Level = highscoreValues[1],
                    Experience = highscoreValues[2],
                });
            }

            return retVal;
        }
    }
}

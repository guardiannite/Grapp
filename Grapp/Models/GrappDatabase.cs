using Grapp.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Grapp.Models
{
    public class GrappDatabase
    {
        /// <summary>
        /// Gets the most recent highscore of a player, otherwise null
        /// </summary>
        public static List<Skill> LatestHighscoreQuery(string playerName, out DateTime? highscoreDate)
        {
            highscoreDate = null;
            using(var db = new GrappContext())
            {
                var player = db.Players.Where(p => p.Name == playerName).FirstOrDefault();
                if(player == null)
                {
                    //No Player, thus this person can't have any highscores
                    return null;
                }

                var highscore = player.Highscores.OrderBy(h => h.Date).LastOrDefault();
                if(highscore == null)
                {
                    //A player exists, but doesn't have any highscores
                    return null;
                }

                highscoreDate = highscore.Date;

                return highscore.Skills.Select(s => Skill.FromEntity(s)).ToList();
            }
        }

        /// <summary>
        /// Records a new highscore row for the player
        /// </summary>
        public static async Task InsertHighscoreAsync(List<Skill> skills, string playerName)
        {
            using(var context = new GrappContext())
            {
                int skillCount = context.SkillEnums.Count();
            
                //Verify the number of skills passed in matches the database
                if(skills.Count != skillCount)
                {
                    throw new ArgumentException(String.Format("Expecting ({0}) skills of highscore data, but received ({1})", skillCount, skills.Count));
                }

                //Grab our player
                var player = context.Players.Where(p => p.Name == playerName).FirstOrDefault();

                //If player doesn't exist, create a new player
                //Make the highscore set to UTC time
                var highscoreEntry = new HighscoreEntity()
                {
                    Player = player ?? new PlayerEntity{ Name = playerName },
                    Date = DateTime.Now.ToUniversalTime(),
                };

                //Create a skill row for each skill
                for(int i = 0; i < skillCount; i++)
                {
                    //LINQ fails when the enumIndex statement is embedded in the query, so this must be separated
                    var enumIndex = (int)skills[i].SkillType;
                    var skillName = context.SkillEnums.Where(s => s.OrderIndex == enumIndex).Select(se => se.Name).FirstOrDefault();
                    var skill = new SkillEntity()
                    {
                        Rank = skills[i].HighscoreRank,
                        Experience = skills[i].Experience,
                        Level = skills[i].Level,
                        Name = skillName,
                    };

                    highscoreEntry.Skills.Add(skill);
                }

                context.Highscores.Add(highscoreEntry);
                await context.SaveChangesAsync();
            }
        }
    }
}
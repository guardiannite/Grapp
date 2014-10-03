using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Grapp.Models
{
    public class GrappDatabase
    {
        public int SkillCount { get; private set;}

        public GrappDatabase()
        {
            using(var context = new GrappEntities())
            {
                SkillCount = context.SkillEnums.Count();
            }
        }
        
        public List<Skill> QueryLatestHighscore(string playerName, out DateTime highscoreDate)
        {
            highscoreDate = new DateTime(2000, 1, 1);
            using(var context = new GrappEntities())
            {
//                string query = @"select h2.Id
//from Player p2
//inner join Highscore h2 on h2.PlayerId = p2.Id
//where h2.Date = (select MAX(h.Date)
//				from Player p
//				inner join Highscore h on h.PlayerId = p.Id
//				where p.Id = h2.PlayerId) 
//and p2.Name = @playerName";
//                var highscoreId = context.Database.SqlQuery<int>(query, new SqlParameter("@playerName", playerName));

//                if(highscoreId == null)
//                {
//                    return new List<Skill>();
//                }

//                return context.Skills.Where<Skill>(s => s.HighscoreId == highscoreId.First()).ToList<Skill>();
                var player = context.Players.Where<Player>(p => p.Name == playerName).FirstOrDefault();
                if(player == null)
                {
                    //No Player, thus this person can't have any highscores
                    return new List<Skill>();
                }

                var highscore = context.Highscores.Where(h => h.PlayerId == player.Id).OrderBy(h => h.Date).AsEnumerable().LastOrDefault<Highscore>();
                if(highscore == null)
                {
                    //A player exists, but doesn't have any highscores
                    return new List<Skill>();
                }

                highscoreDate = highscore.Date;

                var skills = context.Skills.Where(s => s.HighscoreId == highscore.Id).ToList<Skill>();
                if(skills == null)
                {
                    //Someone probablly screwed around with the database and deleted skill entries, but forgot to remove the highscore entry
                    return new List<Skill>();
                }
                return skills;
            }
        }

        public void InsertHighscore(List<HighscoreParser.DeserializedData> skills, string playerName)
        {
            if(skills.Count != SkillCount)
            {
                throw new ArgumentException(String.Format("Expecting ({0}) skills of highscore data, but received ({1})", SkillCount, skills.Count));
            }
            using(var context = new GrappEntities())
            {
                var player = context.Players.Where<Player>(p => p.Name == playerName).FirstOrDefault();

                var highscoreEntry = new Highscore()
                {
                    Player = player ?? new Player() { Name = playerName },
                    Date = DateTime.Now.ToUniversalTime(),
                };

                //a) You set the skill.Highscore property AND add it to the context 
                //OR
                //b) You set the highscoreEntry.Skills property equal to the collection
                for(int i = 0; i < SkillCount; i++)
                {
                    var skill = new Skill()
                    {
                        Rank = skills[i].Rank,
                        Experience = skills[i].Experience,
                        Level = skills[i].Level,
                        Name = context.SkillEnums.Where<SkillEnum>(s=> s.OrderIndex == i).Select(se => se.Name).FirstOrDefault(),
                    };

                    highscoreEntry.Skills.Add(skill);
                }

                context.Highscores.Add(highscoreEntry);
                context.SaveChanges();
            }
        }
    }
}
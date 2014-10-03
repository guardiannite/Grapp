using Grapp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Grapp.Controllers
{
    public class CharacterHistoryController : Controller
    {
        // GET: CharacterHistory
        public ActionResult Index(string playerName)
        {
            var model = new CharacterHistoryModel();
    
            if (playerName != null)
            {
                var serializedData = RequestHighscore(playerName);
                var data = HighscoreParser.GetSkills(serializedData);
                GrappDatabase db = new GrappDatabase();
                DateTime date;
                var previousHighscores = db.QueryLatestHighscore(playerName, out date);

                //for right now, we only want to insert into the player doesn't have a highscore entry from the past day
                if (date < DateTime.Now.ToUniversalTime().AddDays(-1))
                {
                    db.InsertHighscore(data, playerName);
                }
                else
                {
                    //Otherwise, we check to see if any skills' experience has changed since the last db write
                    for(int i = 0; i < previousHighscores.Count; i++)
                    {
                        if(data.ElementAt(i).Experience != previousHighscores.ElementAt(i).Experience)
                        {
                            db.InsertHighscore(data, playerName);
                            break;
                        }
                    }
                }

                model.Highscores = db.QueryLatestHighscore(playerName, out date);

                //TODO: On first highscore entries, the previousHighscores.Count is zero, that means the page won't display, since it picks the Math.Min(current.Count, past.Count)
                var skillIncreases = new List<Skill>();

                for (int i = 0; i < previousHighscores.Count; i++ )
                {
                    var skill = new Skill();
                    skill.Level = model.Highscores.ElementAt(i).Level - previousHighscores.ElementAt(i).Level;
                    skill.Name = previousHighscores.ElementAt(i).Name;
                    skill.Rank = model.Highscores.ElementAt(i).Rank - previousHighscores.ElementAt(i).Rank;
                    skill.Experience = model.Highscores.ElementAt(i).Experience - previousHighscores.ElementAt(i).Experience;

                    skillIncreases.Add(skill);
                }

                model.SkillIncrease = skillIncreases;
            }
            return View(model);
        }

        public string RequestHighscore(string playerName)
        {
            string url = String.Format("http://services.runescape.com/m=hiscore/index_lite.ws?player={0}", playerName);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            StreamReader resStream = new StreamReader(response.GetResponseStream());

            return resStream.ReadToEnd();
        }
    }
}
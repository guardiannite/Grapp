using Grapp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Grapp.Controllers
{
    public class CharacterHistoryController : Controller
    {
        // GET: CharacterHistory
        public async Task<ActionResult> Index(string playerName)
        {
            var model = new CharacterHistoryModel();
    
            if (playerName != null)
            {
                var rsCurrentHighscores = await RunescapeClient.RequestHighscoreAsync(playerName);

                if(rsCurrentHighscores == null)
                {
                    //User doesn't exist in Runescape's highscores
                    model.ErrorMessage = "Failed to find user in Runescape's highscores";
                    return View(model);
                }

                DateTime? date;
                var previousHighscores = GrappDatabase.LatestHighscoreQuery(playerName, out date);

                if(previousHighscores == null)
                {
                    //No highscores were found for this user in our database
                    previousHighscores = Skill.EmptyRecord();
                    date = DateTime.Now;
                }

                //Date of the most recent highscore entry in our database
                model.LastUpdate = date.Value;
                model.Highscores = previousHighscores;
                model.CurrentUpdate = date.Value;
                bool updatedUser = false;

                //for right now, we only want to insert if the player doesn't have a highscore entry from the past day
                if (date < DateTime.Now.ToUniversalTime().AddDays(-1))
                {
                    await GrappDatabase.InsertHighscoreAsync(rsCurrentHighscores, playerName);
                    updatedUser = true;
                }
                else
                {
                    //Otherwise, we check to see if any skills' experience has changed since the last db write
                    for(int i = 0; i < previousHighscores.Count; i++)
                    {
                        if (rsCurrentHighscores[i].Experience != previousHighscores[i].Experience)
                        {
                            await GrappDatabase.InsertHighscoreAsync(rsCurrentHighscores, playerName);
                            updatedUser = true;
                            break;
                        }
                    }
                }

                if(updatedUser)
                {
                    model.Highscores = rsCurrentHighscores;
                    model.CurrentUpdate = DateTime.UtcNow;
                }

                //TODO: On first highscore entries, the previousHighscores.Count is zero, that means the page won't display, since it picks the Math.Min(current.Count, past.Count)
                var skillIncreases = new List<Skill>();

                for (int i = 0; i < previousHighscores.Count; i++ )
                {
                    var skill = new Skill
                    {
                        Level = model.Highscores[i].Level - previousHighscores[i].Level,
                        SkillType = previousHighscores[i].SkillType,
                        HighscoreRank = model.Highscores[i].HighscoreRank - previousHighscores[i].HighscoreRank,
                        Experience = model.Highscores[i].Experience - previousHighscores[i].Experience,
                    };
                    skillIncreases.Add(skill);
                }

                model.SkillIncrease = skillIncreases;
            }
            return View(model);
        }

    }
}
﻿using Grapp.Models;
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
                db.InsertHighscore(data, playerName);

                DateTime date;
                model.Highscores = db.QueryLatestHighscore(playerName, out date) ;
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
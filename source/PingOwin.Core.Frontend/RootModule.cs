using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Responses;
using PingIt.Lib;
using PingOwin;

namespace PingIt.Cmd.WebHost
{
    public class RootModule : NancyModule
    {
        private readonly IPenguinResultsRepository _resultsRepo;
        private readonly IPenguinRepository _penguinRepository;

        public RootModule(IPenguinRepository penguinRepository, IPenguinResultsRepository resultsRepo)
        {
            _resultsRepo = resultsRepo;
            Get["/"] = x => new RedirectResponse("/pingowins", RedirectResponse.RedirectType.Temporary);
            
            Get["/pingowins", true] = async (x, t) =>
            {
                var all = await penguinRepository.GetAll();
                var allPenguinsModel = new PingOwinsModel();
                var pengs = all.Select(c => new SinglePingOwin {Url = c.Url});
                allPenguinsModel.Penguins = pengs;
                return View["AllPingowins.sshtml", allPenguinsModel];
            };

            Get["/results", true] = async (x,t) =>
            {
                var allResults = await _resultsRepo.GetAll();
                var resultsModel = new PingResultsModel();
                var results = allResults.Select(c =>  new SingleResult {Url = c.Url, ResponseTime = c.ResponseTime.ToString()});
                resultsModel.Results = results;
                return View["Results.sshtml", resultsModel];
            };
        }
    }

    public class PingOwinsModel 
    {
        public IEnumerable<SinglePingOwin> Penguins { get; set; } 
    }

    public class SinglePingOwin
    {
        public string Url { get; set; }
    }
    public class PingResultsModel
    {
        public IEnumerable<SingleResult> Results { get; set; }
    }

    public class SingleResult
    {
        public string Url { get; set; }
        public string ResponseTime { get; set; }
    }
}
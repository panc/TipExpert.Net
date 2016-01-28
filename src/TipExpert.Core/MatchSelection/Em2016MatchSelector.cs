using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TipExpert.Core.MatchSelection
{
    public class Em2016MatchSelector : IMatchSelector
    {
        public const string LEAGUE_GROUP_A = "EM-2016-Group-A";
        public const string LEAGUE_GROUP_B = "EM-2016-Group-B";
        public const string LEAGUE_GROUP_C = "EM-2016-Group-C";
        public const string LEAGUE_GROUP_D = "EM-2016-Group-D";
        public const string LEAGUE_GROUP_E = "EM-2016-Group-E";
        public const string LEAGUE_GROUP_F = "EM-2016-Group-F";
        public const string LEAGUE_ROUND_OF_LAST_16 = "EM-2016-RoundOfLast16";
        public const string LEAGUE_QUATER_FINAL = "EM-2016-QuaterFinal";
        public const string LEAGUE_SEMI_FINAL = "EM-2016-SemiFinal";
        public const string LEAGUE_FINAL = "EM-2016-Final";

        private readonly ILeagueStore _leagueStore;
        private readonly IMatchStore _matchStore;

        public Em2016MatchSelector(ILeagueStore leagueStore, IMatchStore matchStore)
        {
            _leagueStore = leagueStore;
            _matchStore = matchStore;
        }

        public async Task<List<Match>> GetMatches(string matchesMetadata)
        {
            // metadata sample:
            // "{\"groupA\":true,\"groupB\":false,\"groupC\":false,\"groupD\":false,\"groupE\":false,\"groupF\":false,\"roundOfLast16\":false,\"quaterFinal\":false,\"semiFinal\":false,\"final\":false}";

            var leagues = await _leagueStore.GetAll();

            var groupDefinition = JsonConvert.DeserializeObject<GroupDefinition>(matchesMetadata);

            var list = new List<Match>();
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.groupA, LEAGUE_GROUP_A));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.groupB, LEAGUE_GROUP_B));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.groupC, LEAGUE_GROUP_C));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.groupD, LEAGUE_GROUP_D));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.groupE, LEAGUE_GROUP_E));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.groupF, LEAGUE_GROUP_F));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.roundOfLast16, LEAGUE_ROUND_OF_LAST_16));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.quaterFinal, LEAGUE_QUATER_FINAL));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.semiFinal, LEAGUE_SEMI_FINAL));
            list.AddRange(await _GetMatchesForLeague(leagues, groupDefinition.final, LEAGUE_FINAL));

            return list;
        }

        private async Task<Match[]> _GetMatchesForLeague(League[] leagues, bool addGroup, string leagueName)
        {
            if (!addGroup)
                return new Match[0];

            var league = leagues.FirstOrDefault(x => x.Name == leagueName);
            if (league == null)
                return new Match[0];

            return await _matchStore.GetMatchesForLeague(league.Id);
        }

        private class GroupDefinition
        {
            public bool groupA { get; set; }
            public bool groupB { get; set; }
            public bool groupC { get; set; }
            public bool groupD { get; set; }
            public bool groupE { get; set; }
            public bool groupF { get; set; }
            public bool roundOfLast16 { get; set; }
            public bool quaterFinal { get; set; }
            public bool semiFinal { get; set; }
            public bool final { get; set; }
        }
    }
}
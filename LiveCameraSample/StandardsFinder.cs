using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using Bsi.Ipp.SearchIndexing.Common;

namespace LiveCameraSample
{
    public class StandardsFinder
    {
        public string GetStandards(string keywordForSearch)
        {
            var relevantSearchHits = SearchClient.Instance.Search<BibliographicInformation>(Language.English)
                        .For(keywordForSearch)
                        .InField(x => x.Identifier, 5)
                        .InField(x => x.DisplayName, 5)
                        .InField(x => x.DisplayNameFormatted, 5)
                        .InField(x => x.EndecaTitle, 0.5)
                        .GetResult();
            if (relevantSearchHits.Any())
               return relevantSearchHits.FirstOrDefault().DisplayName;
            return string.Empty;
        }
    }
}

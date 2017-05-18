using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTrade.App.Models
{
    public class PageInfo
    {
        public PageInfo(int totalCount, int pageIndex, int pageSize)
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get { return (int)Math.Ceiling(TotalCount / (double)PageSize); } }

        public IEnumerable<string> GetPageTokens()
        {
            if (PageCount <= 10)
                return Enumerable.Range(1, PageCount).Select(i => i.ToString());

            var pageTokens = new List<string>();
            pageTokens.Add("1");

            var pageNumber = PageIndex + 1;
            for (int i = 2; i < PageCount; i++)
            {
                if (i <= pageNumber + 1 && i >= pageNumber - 1)
                {
                    pageTokens.Add($"{i}");
                }
                else if (pageNumber == 1 && i == 3)
                {
                    pageTokens.Add($"{i}");
                    pageTokens.Add($"...");
                }
                else if (pageNumber == PageCount && i == PageCount - 2)
                {
                    pageTokens.Add($"...");
                    pageTokens.Add($"{i}");
                }
                else if (i == pageNumber - 2 || i == pageNumber + 2)
                {
                    pageTokens.Add($"...");
                }
            }

            pageTokens.Add($"{PageCount}");

            return pageTokens;
        }
    }
}

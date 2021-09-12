using System;
using System.Collections.Generic;
using System.Linq;
namespace MongoRepository.Models
{
    public class PagedResult<T> : PagingInfo
    {
        public List<T> ResultSet { get; set; }

        public PagedResult(List<T> items, long count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize) - 1;

            ResultSet = items;
        }

        public static PagedResult<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();

            if (pageNumber < 0)
                pageNumber = 0;
            var items = source.Skip((pageNumber) * pageSize).Take(pageSize).ToList();

            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }

        public static PagedResult<T> ToPagedList(IEnumerable<T> source, long count, int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
                pageNumber = 0;
            var items = source.Skip((pageNumber) * pageSize).Take(pageSize).ToList();

            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }
    }
}
using SmartAirControl.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAirControl.API.Core.Pagination
{
    public static class PaginationUtils
    {
        public static PagedResponse<T> CreatedPagedResponse<T>(IEnumerable<T> records, int pageSize, int page) where T : new()
        {
            var result = new PagedResponse<T>();

            result.Page = page;
            result.PageSize = pageSize;
            result.TotalRecords = records.Count();
            result.TotalPages = (int)Math.Ceiling(result.TotalRecords / (double)pageSize);

            var filtered = records.Skip((page - 1) * pageSize)
                                  .Take(pageSize);

            result.Data = filtered;

            return result;
        }
    }
}

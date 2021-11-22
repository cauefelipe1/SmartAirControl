using System.Collections.Generic;

namespace SmartAirControl.Models.Base
{
    /// <summary>
    /// Defines a generic paged list response.
    /// </summary>
    /// <typeparam name="T">Type of the list.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// Current list page.
        /// </summary>
        /// <example>1</example>
        public int Page { get; set; }

        /// <summary>
        /// Current page size.
        /// </summary>
        /// <example>100</example>
        public int PageSize { get; set; }

        /// <summary>
        /// Total of pages based on <see cref="PageSize"/>.
        /// </summary>
        /// <example>10</example>
        public int TotalPages { get; set; }

        /// <summary>
        /// General total of record.
        /// Takes into consideration all pages.
        /// </summary>
        /// <example>1000</example>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Page data
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public PagedResponse() { }
    }
}

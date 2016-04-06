using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TeknoMobi.Common.Mvc
{
    public interface IPagedList {
        int TotalCount { get; set; }
        int TotalPages { get; set; }
        int PageIndex { get; set;}
        int PageSize { get; set;}
        bool IsPreviousPage { get; }
        bool IsNextPage { get; }
    }

    public class PagedList<T> : List<T>, IPagedList {
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize) {

            int total = source.Count();
            this.TotalCount = total;
            this.TotalPages = total / pageSize;
            
            if (total % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        public PagedList(List<T> source, int pageIndex, int pageSize)
        {

            int total = source.Count();
            this.TotalCount = total;
            this.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source.Skip((pageIndex-1) * pageSize).Take(pageSize).ToList());
        }

        public int TotalPages {
            get;
            set;
        }

        public int TotalCount {
            get;
            set;
        }

        public int PageIndex {
            get;
            set;
        }

        public int PageSize {
            get;
            set;
        }

        public bool IsPreviousPage {
            get {
                return (PageIndex > 1);
            }
        }

        public bool IsNextPage {
            get {
                return (PageIndex * PageSize) <= TotalCount;
            }
        }
    }

    public static class Pagination {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize) {
            return new PagedList<T>(source, index, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index) {
            return new PagedList<T>(source, index, 10);
        }
    }
}
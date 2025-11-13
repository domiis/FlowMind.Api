// Helpers/PaginationHelper.cs
using FlowMind.Api.DTOs.Links;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Api.Helpers
{
    public static class PaginationHelper
    {
        public static object CreatePaginatedResponse<T>(
            List<T> items,
            int page,
            int pageSize,
            int total,
            IUrlHelper urlHelper,
            string routeName,
            object? routeValues = null)
        {
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var links = new List<LinkDto>
            {
                new LinkDto
                {
                    Href = urlHelper.Link(routeName, routeValues ?? new { page, pageSize })!,
                    Rel = "self",
                    Method = "GET"
                }
            };

            if (page < totalPages)
            {
                links.Add(new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { page = page + 1, pageSize })!,
                    Rel = "next",
                    Method = "GET"
                });
            }

            if (page > 1)
            {
                links.Add(new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { page = page - 1, pageSize })!,
                    Rel = "previous",
                    Method = "GET"
                });
            }

            return new
            {
                page,
                pageSize,
                total,
                totalPages,
                links,
                items
            };
        }
    }
}




﻿using System.Linq;

namespace Battles.Application.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<T> GrabSegment<T>(this IQueryable<T> @this, int index, int size = 5) =>
            @this.Skip(index * size)
                .Take(size);
    }
}
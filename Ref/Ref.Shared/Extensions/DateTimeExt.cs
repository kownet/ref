﻿using System;

namespace Ref.Shared.Extensions
{
    public static class DateTimeExt
    {
        public static string Format(this DateTime dt)
            => dt.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
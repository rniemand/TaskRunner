﻿using System;
using Serilog;
using Serilog.Configuration;

namespace TaskBuilder.Common.Logging
{
  public static class LoggingExtensions
  {
    public static LoggerConfiguration WithCaller(this LoggerEnrichmentConfiguration enrich)
    {
      if (enrich == null)
        throw new ArgumentNullException(nameof(enrich));

      return enrich.With<CallerEnricher>();
    }
  }
}
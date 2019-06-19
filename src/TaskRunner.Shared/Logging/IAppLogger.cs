﻿namespace TaskRunner.Shared.Logging
{
  public interface IAppLogger
  {
    // Verbose
    void Verbose<T1>(string template, T1 p1);
    void Verbose<T1, T2>(string template, T1 p1, T2 p2);
    void Verbose<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3);


    // Debug
    void Debug(string template);
    void Debug<T>(string template, T p1);
    void Debug<T1, T2>(string template, T1 p1, T2 p2);
    void Debug<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3);


    // Info
    void Info(string template);
    void Info<T>(string template, T p1);
    void Info<T1, T2>(string template, T1 p1, T2 p2);
    void Info<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3);


    // Warn
    void Warn<T1, T2>(string template, T1 p1, T2 p2);
    void Warn<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3);
    void Warn<T1, T2, T3, T4>(string template, T1 p1, T2 p2, T3 p3, T4 p4);


    // Error
    void Error(string template);
    void Error<T1>(string template, T1 p1);
    void Error<T1, T2>(string template, T1 p1, T2 p2);
    void Error<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3);
  }
}

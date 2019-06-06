namespace TaskBuilder.Common.Logging.Interfaces
{
  public interface IAppLogger
  {
    // Debug
    void Debug(string message);
    void Debug<T>(string message, T p1);
    void Debug<T1, T2>(string message, T1 p1, T2 p2);

    // Info
    void Info(string message);
    void Info<T>(string message, T p1);
    void Info<T1, T2>(string message, T1 p1, T2 p2);

    // Warn
    void Warn<T1, T2>(string message, T1 p1, T2 p2);
    void Warn<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3);
    void Warn<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4);


    // Error
    void Error<T1>(string message, T1 p1);
    void Error<T1, T2>(string message, T1 p1, T2 p2);
  }
}

namespace MinApiOnNet.Services;

public static class ExceptionsService
{
    public static async void AsyncVoid()
    {
        Console.WriteLine("Entering async void method");
        await AsyncTask();

        // Pretend there's some super critical code right here
        // ...

        Console.WriteLine("Leaving async void method.");
    }

    public static async Task AsyncTask()
    {
        Console.WriteLine("Entering async Task method");
        Console.WriteLine("About to throw...");
        await Task.CompletedTask;
        throw new Exception("The expected exception");
    }
}
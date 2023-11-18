namespace MinApiOnNet.AsyncExtensions;

public class AppState
{
    public event EventHandler? DemoEvent;

    public void Invoke()
    {
        DemoEvent?.Invoke(this, EventArgs.Empty);
        Console.WriteLine("All handlers have been executed!");
    }

    public void InvokeAsync()
    {
        //DemoEvent?.Invoke(this, EventArgs.Empty);
        DemoEvent?.InvokeAsync(this, EventArgs.Empty).GetAwaiter().GetResult();
        Console.WriteLine("All handlers have been executed!");
    }
}
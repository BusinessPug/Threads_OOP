namespace Threads_OOP;

internal class Counter
{
    private int _value = 0;

    public void IncrementInterlocked()
    {
        Interlocked.Increment(ref _value);
    }

    public void Increment()
    {
        _value++;
    }

    public void InterlockedUpTo(int max)
    {
        for (int i = 0; i < max; i++)
        {
            IncrementInterlocked();
        }
    }

    public int GetValue()
    {
        return _value;
    }
}

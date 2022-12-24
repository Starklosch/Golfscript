namespace Golfscript
{
    public interface IResignable<T> where T : class
    {
        public T Resign();
    }
}

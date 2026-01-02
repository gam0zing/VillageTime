public abstract class Singleton<T> where T : Singleton<T>, new() {
    private static T instance;

    protected Singleton() {}

    public static T GetInstance() {
        return Singleton<T>.instance;
    }
}
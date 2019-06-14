using CQRS.EventSourcing.CRM.Domain.Actions;

public delegate TEntity Reducer<TEntity>(TEntity state, IAction action) where TEntity : class;

public class ReduxStore<TEntity> where TEntity : class
{
    readonly Reducer<TEntity> reducer;

    public TEntity State { get; private set; }

    public delegate void SubscribeDelegate(TEntity state);
    public event SubscribeDelegate Subscribe;

    public ReduxStore(Reducer<TEntity> reducer, TEntity initialState)
    {
        this.reducer = reducer;
        State = initialState;
    }

    public IAction Dispatch(IAction action)
    {
        State = reducer(State, action);
        Subscribe?.Invoke(State);

        return action;
    }
}
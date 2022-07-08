/// <summary>
/// Implemented by actions that serve as alternates to other actions
/// </summary>
public interface IAltAction
{
    BaseAction GetRootAction();
}

using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    [SerializeField] private Pathfinding pathfinder = null;

    private void OnEnable()
    {
        DestructableObject.DestroyedStatic += UpdatePathfinder;
    }

    private void OnDisable()
    {
        DestructableObject.DestroyedStatic += UpdatePathfinder;
    }

    private void UpdatePathfinder(GridPosition gridPosition)
    {
        pathfinder.SetIsWalkable(gridPosition, true);
    }
}
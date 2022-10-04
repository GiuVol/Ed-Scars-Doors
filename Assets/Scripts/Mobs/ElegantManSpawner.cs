using System.Collections;
using UnityEngine;

public class ElegantManSpawner : EventTrigger
{
    private const string ElegantManResourcePath = "Mobs/ElegantMan";

    [SerializeField]
    private Transform _spawnPoint;

    protected override IEnumerator Action()
    {
        if (_spawnPoint == null)
        {
            yield break;
        }

        ElegantMan elegantManResource = Resources.Load<ElegantMan>(ElegantManResourcePath);

        if (elegantManResource != null)
        {
            Instantiate(elegantManResource, _spawnPoint.position, Quaternion.identity);
        }
    }
}

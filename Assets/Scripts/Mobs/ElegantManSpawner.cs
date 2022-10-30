using System.Collections;
using UnityEngine;

public class ElegantManSpawner : EventTrigger
{
    private const string ElegantManResourcePath = "Mobs/ElegantMan";

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private PatrolPointsGroup _ppGroup;

    private void Start()
    {
        if (_ppGroup != null)
        {
            _ppGroup.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator Action(PlayerController player)
    {
        if (_spawnPoint == null)
        {
            yield break;
        }

        ElegantMan elegantManResource = Resources.Load<ElegantMan>(ElegantManResourcePath);

        if (elegantManResource != null)
        {
            ElegantMan elegantMan = Instantiate(elegantManResource, _spawnPoint.position, Quaternion.identity);

            if (_ppGroup != null)
            {
                _ppGroup.gameObject.SetActive(true);
                elegantMan.PPGroup = _ppGroup;
            }
        }
    }
}

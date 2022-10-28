using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : EventTrigger
{
    [SerializeField]
    private GenericMob _toSpawn;

    [SerializeField]
    private Transform _spawnPosition;

    protected override IEnumerator Action(PlayerController player)
    {
        if (_toSpawn != null && _spawnPosition != null)
        {
            GenericMob boss = Instantiate(_toSpawn, _spawnPosition.position, Quaternion.identity);

            float lerpFactor = 0;
            boss.transform.localScale = Vector3.zero;

            do
            {
                lerpFactor = Mathf.Clamp01(lerpFactor + Time.fixedDeltaTime);
                boss.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpFactor);

                yield return new WaitForFixedUpdate();

            } while (lerpFactor < 1);
        }
    }
}

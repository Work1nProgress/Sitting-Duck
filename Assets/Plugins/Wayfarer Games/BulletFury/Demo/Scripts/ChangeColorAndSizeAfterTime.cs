using System.Collections;
using System.Collections.Generic;
using BulletFury;
using UnityEngine;

public class ChangeColorAndSizeAfterTime : MonoBehaviour
{
    [SerializeField] private BulletManager manager;

    [SerializeField] private float newSize = 0.5f;
    [SerializeField] private Color newColor = Color.white;
    [SerializeField] private float newArc = 90f;

    [SerializeField] private float waitTime;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(waitTime);

        var bulletSettings = manager.GetBulletSettings();
        bulletSettings.SetSize(newSize);
        bulletSettings.SetStartColor(newColor);
        
        var spawnSettings = manager.GetSpawnSettings();
        spawnSettings.SetArc(newArc);
    }
}

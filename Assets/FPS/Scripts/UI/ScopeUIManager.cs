using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

public class ScopeUIManager : MonoBehaviour
{
    public GameObject scopeUI;
    private PlayerWeaponsManager pw;
    // Start is called before the first frame update
    void Start()
    {
        pw = FindObjectOfType<PlayerWeaponsManager>();
        pw.OnScopedWeapon += OnScope;
        pw.OffScopedWeapon += OffScope;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnScope()
    {
        scopeUI.SetActive(true);
    }
    public void OffScope()
    {
        scopeUI.SetActive(false);
    }
}

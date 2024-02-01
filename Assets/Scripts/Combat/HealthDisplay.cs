using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health _health = null;
    [SerializeField] private GameObject _healthBarParent = null;
    [SerializeField] private Image _healthBarImage = null;

    private void Awake()
    {
        _health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        _health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        _healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        _healthBarParent.SetActive(false);
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        _healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

}

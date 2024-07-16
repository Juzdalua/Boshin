using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    [SerializeField] private TextMesh damageText;

    void Update()
    {
        FadeoutDamage();
    }

    public void DisplayDamage(float damage, bool isCritical)
    {
        damageText.text = isCritical ? "<color=#ff0000>" + damage + "</color>" : "<color=#ffffff>" + damage + "</color>";
    }

    private void FadeoutDamage()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 2f);
        Destroy(gameObject, 1f);
    }
}

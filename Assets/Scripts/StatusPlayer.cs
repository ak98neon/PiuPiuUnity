/*
 * @author Artem Kudrya
 * @date 05.11.2016
 * @see Назначение: Скрипт который отвечает за инвентарь игрока, жизни, броню
 */

using UnityEngine;
using System.Collections;

public class StatusPlayer : MonoBehaviour
{
    [SerializeField]
    private string id;
    [SerializeField]
    private int hpPlayer = 10;
    [SerializeField]
    private int armorPlayer;

    public int statusHpPlayer { get; set; }
    public int statusArmorPlayer { get; set; }
    public string Id { get => id; set => id = value; }

    void Update()
    {
        alive();
    }

    public void hpPlayerDamage(int damage)
    {
        if (this.armorPlayer > 0)
        {
            armorPlayerDamage(damage);
        } else
        {
            this.hpPlayer -= damage;
        }
    }

    public void armorPlayerDamage(int damage)
    {
        this.armorPlayer -= damage;
    }

    public void alive()
    {
        if (hpPlayer <= 0)
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using System.Collections;/// <remarks>
/// Shooting script
/// </remarks>
public class Shooting : MonoBehaviour
{
    #region Variables
    public enum CurrentWeapon { none, pistol, rifle } //Status of active gun
    private CurrentWeapon currentGun;

    public enum menuAlive { game = 0, setting = 1 }; //Status of menu
    public menuAlive polKursor;

    public Rigidbody bulletPrefab;
    public Transform rukiPlayer;
    public float bulletForce = 50000.0f;

    public Camera mainCamera; //Main camera
    public ParticleSystem iskra;
    public AudioSource audio;
    public AudioClip firePistol;
    public AudioClip reloadAudio;

    [SerializeField]
    private int damageHP = 1;

    [SerializeField]
    private int spawnCountAmmoPistol = 8;
    private int allAmmoPistol = 30;
    private int ammoPistol = 8;

    [SerializeField]
    private int spawnCountAmmoRifle = 30;
    private int allAmmoRifle = 90;
    private int ammoRifle = 30;

    [SerializeField]
    public Transform ruki;
    private float forceGrenade = 500.0f;

    public bool reloadActive = false;
    public float timeReload = 3.0f;

    public Texture2D crossHair;
    private Rect positionCrosshair;

    //Animation
    private Animator _animator;

    private MultiListener listener;
    private string respawnTag = "Respawn";

    public CurrentWeapon CurrentGun { get => currentGun; set => currentGun = value; }
    #endregion

    /// <summary>
    /// Standart method in Unity, launched at the start of the script
    /// </summary>
    void Start()
    {
        _animator = GetComponent<Animator>();

        positionCrosshair = new Rect((Screen.width - crossHair.width) / 2, (Screen.height - crossHair.height) / 2, crossHair.width, crossHair.height);

        bulletPrefab.GetComponent<Rigidbody>();
        
        ammoPistol = spawnCountAmmoPistol;
        ammoRifle = spawnCountAmmoRifle;

        CurrentGun = CurrentWeapon.none;
        polKursor = menuAlive.game;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        listener = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<MultiListener>();
    }

    /// <summary>
    /// Standart method in Unity, it called each frame/sec
    /// </summary>
    void Update()
    {
        activeSettings();
        vuborGun();
        FirePistol();
        FireRifle();
        reload();
    }

    /// <summary>
    /// Panel settings, open/close
    /// </summary>
    public void activeSettings()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && polKursor == menuAlive.game)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameObject.GetComponent<MouseLook>().enabled = false;
            gameObject.GetComponent<FPSInput>().enabled = false;
            mainCamera.GetComponent<MouseLook>().enabled = false;
            polKursor = menuAlive.setting;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && polKursor == menuAlive.setting)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameObject.GetComponent<MouseLook>().enabled = true;
            gameObject.GetComponent<FPSInput>().enabled = true;
            mainCamera.GetComponent<MouseLook>().enabled = true;
            polKursor = menuAlive.game;
        }
    }

    /// <summary>
    /// Switch gun
    /// </summary>
    public void vuborGun()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            CurrentGun = CurrentWeapon.pistol;
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            CurrentGun = CurrentWeapon.rifle;
        }
    }

    /// <summary>
    /// Resposible for fire with pistol
    /// </summary>
    public void FirePistol()
    {
        if (currentGun != CurrentWeapon.none)
        {
            rukiPlayer.rotation = mainCamera.GetComponent<Transform>().rotation;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)
            && reloadActive == false
            && ammoPistol > 0
            && polKursor == menuAlive.game
            && CurrentGun == CurrentWeapon.pistol)
        {
            audio.PlayOneShot(firePistol);
            ammoPistol--;

            Vector3 point = new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, 0);
            Ray ray = mainCamera.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                ReactiveTarget target = hitObject.GetComponent<ReactiveTarget>();
                StatusPlayer player = hitObject.GetComponent<StatusPlayer>();

                listener.shoot(ClientAction.SHOOT, hit.point);

                if (target != null)
                {
                    listener.shoot(ClientAction.SHOOT, hit.point);
                    target.ReactToHit();
                }
                else
                {
                    //iskra.transform.position = hit.point;
                    //iskra.Play();
                    if (player != null)
                    {
                        StatusPlayer status = hitObject.GetComponent<StatusPlayer>();
                        listener.hitPlayer(ClientAction.HIT, status.Id, hit.point);
                        player.hpPlayerDamage(damageHP);
                    }
                    else
                    {
                        StartCoroutine(ShootingUtil.SphereIndicator(hit.point));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resposible for fire with rifle
    /// </summary>
    public void FireRifle()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && reloadActive == false && ammoRifle > 0 && polKursor == menuAlive.game && CurrentGun == CurrentWeapon.rifle)
        {
            audio.PlayOneShot(firePistol);
            ammoRifle--;
            int posx = Random.Range(1, 10);
            int posy = Random.Range(1, 10);
            Vector3 point = new Vector3(mainCamera.pixelWidth / 2 - posx + posy, mainCamera.pixelHeight / 2 + posx - posy, 0);
            Ray ray = mainCamera.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                ReactiveTarget target = hitObject.GetComponent<ReactiveTarget>();
                StatusPlayer player = hitObject.GetComponent<StatusPlayer>();

                if (target != null)
                {
                    target.ReactToHit();
                }
                else
                {
                    iskra.transform.position = hit.point;
                    iskra.Play();
                    if (player != null)
                    {
                        player.hpPlayerDamage(damageHP);
                    }
                    else
                    {
                        StartCoroutine(ShootingUtil.SphereIndicator(hit.point));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resposible for reload weapons
    /// </summary>
    public void reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && CurrentGun == CurrentWeapon.pistol && ammoPistol != spawnCountAmmoPistol && allAmmoPistol > 0)
        {
            audio.PlayOneShot(reloadAudio);
            reloadActive = true;
            StartCoroutine(Time_reload());
            allAmmoPistol += ammoPistol;

            if (allAmmoPistol == spawnCountAmmoPistol)
            {
                ammoPistol = allAmmoPistol;
            }
            else if (allAmmoPistol > spawnCountAmmoPistol)
            {
                ammoPistol = spawnCountAmmoPistol;
                allAmmoPistol -= spawnCountAmmoPistol;
            }
            else if (allAmmoPistol < spawnCountAmmoPistol)
            {
                ammoPistol = allAmmoPistol;
                allAmmoPistol = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && CurrentGun == CurrentWeapon.rifle && ammoRifle != spawnCountAmmoRifle && allAmmoRifle > 0)
        {
            audio.PlayOneShot(reloadAudio);
            reloadActive = true;
            StartCoroutine(Time_reload());
            allAmmoRifle += ammoRifle;

            if (allAmmoRifle == spawnCountAmmoRifle)
            {
                ammoRifle = allAmmoRifle;
            } else if (allAmmoRifle > spawnCountAmmoRifle)
            {
                ammoRifle = spawnCountAmmoRifle;
                allAmmoRifle -= spawnCountAmmoRifle;
            } else if (allAmmoRifle < spawnCountAmmoRifle)
            {
                ammoRifle = allAmmoRifle;
                allAmmoRifle = 0;
            }
        }
    }

    public IEnumerator Time_reload()
    {
        yield return new WaitForSeconds(timeReload);
        reloadActive = false;
    }

    void OnGUI()
    {
        if (mainCamera != null)
        {
            GUI.DrawTexture(positionCrosshair, crossHair);
        }

        if (CurrentGun == CurrentWeapon.pistol)
        {
            GUI.Label(new Rect(5, 30, 100, 30), "Пистолет: [" + ammoPistol + "/" + allAmmoPistol + "]");
        } else if (CurrentGun == CurrentWeapon.rifle)
        {
            GUI.Label(new Rect(5, 30, 120, 30), "Автомат: [" + ammoRifle + "/" + allAmmoRifle + "]");
        }

        if (ammoPistol == 0 && allAmmoPistol > 0 && CurrentGun == CurrentWeapon.pistol) {
            GUI.Label(new Rect(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2 + 200, 150, 30), "Перезарядите [R]");
        } else if (ammoRifle == 0 && allAmmoRifle > 0 && CurrentGun == CurrentWeapon.rifle)
        {
            GUI.Label(new Rect(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2 + 200, 150, 30), "Перезарядите [R]");
        }
    }
}

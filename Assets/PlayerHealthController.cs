using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController Instance;
    [SerializeField] private Slider HealthBar;
    public float MaxHp;
    public float currentHp;
    public int HpPotions;
    public bool Hit;
    public float RestoreHealthSpeed = 0.1f;
    public static event Action HealEvent;
    public static float HealValue;
    public bool isDead;
    SpriteRenderer spriteRend;

    private void Awake()
    {
        Instance = this;
        HealEvent += HealPlayer;
        HealValue = 25;
        HealthBar.maxValue = MaxHp;
        spriteRend = this.gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
      
    }

    void Update()
    {
        if (Hit) HitRecoveryAnim();
        else spriteRend.material.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b,  1);

        if (Input.GetKeyDown(KeyCode.P))
        {
            ReciveDamage(5);
        }

        if (Input.GetMouseButtonDown(2)) HealEvent?.Invoke();
        CheckPlayerHp();

    }

    void CheckPlayerHp()
    {
        isDead = (currentHp <= 0) ? true : false;
    }

    void HealPlayer()
    {
        if(HpPotions > 0)
        {
            HpPotions--;
            StartCoroutine(HealAnimation(1, currentHp, currentHp + HealValue));
        }
    }

    private IEnumerator HealAnimation(float duration, float start, float end)
    {
        HealthBar.value = currentHp;

        float t = 0f;
        while (t < duration)
        {
            var current = Mathf.LerpUnclamped(start, end, t);
            HealthBar.value = Mathf.FloorToInt(current);
            currentHp = current;
            yield return null; 
            t += Time.deltaTime; 
        }
    }

    public void ReciveDamage(float DmgValue)
    {
        currentHp -= DmgValue;
        HealthBar.value = currentHp;
        Hit = true;
        HitRecoveryState();
    }

    public void HitRecoveryState()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Invoke("ExitRecoveryState", 5f);
    }

    void ExitRecoveryState()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        Hit = false;
    }


    private void HitRecoveryAnim()
    {
        var spriteC = spriteRend.color;
        var c = Mathf.PingPong(Time.time, 1f);
        spriteRend.material.color = new Color(spriteC.r, spriteC.g, spriteC.b, c);
    }

}

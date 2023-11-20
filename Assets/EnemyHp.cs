using UnityEngine;
using UnityEngine.Events;

public class EnemyHp : MonoBehaviour
{

    private float currentHp;
    public float maxHp;
    public UnityEvent OnDeath;
    [SerializeField] private Material HitMaterial;
    [SerializeField] private Material NormalMaterial;
    private SpriteRenderer Sr;
    private Transform PlayerPos;
    private Rigidbody2D rb;
    void Awake()
    {
        PlayerPos = GameObject.FindGameObjectWithTag("Player").transform; 
        currentHp = maxHp;
        Sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    void Death()
    {
        if(currentHp <= 0)
        {
            OnDeath.Invoke();
            Destroy(this.gameObject);
        }
    }

    public void ReciveDamage(float dmg)
    {
        currentHp -= dmg;
        Sr.material = HitMaterial;
        Death();
        KnockBack(30f);
        Invoke("ReturnToNormalMaterial", 0.2f);
    }

    void ReturnToNormalMaterial()
    {
        Sr.material = NormalMaterial;
    }

    void KnockBack(float KnBValue)
    {
        if(transform.position.x < PlayerPos.position.x) rb.AddForce(new Vector2(-25f, 45f) * KnBValue * Time.fixedDeltaTime,ForceMode2D.Impulse);
        else rb.AddForce(new Vector2(25f, 45f) * KnBValue * Time.fixedDeltaTime,ForceMode2D.Impulse);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombatManager : MonoBehaviour
{
    Animator playerAnimator;
    ThirdPersonPlayerMovement playerInputs; // ana karakterin hareketi için girdileri algılayan unitynin new ınput sistemi  
    InputAction swordAttackAction;


    [Header("Looking Direction Variables")]
    [SerializeField] Vector3 mousePosition;
    [SerializeField] Vector3 mousePosToWievPos;
    [SerializeField] Vector2 playerPosToWievPos;
    [SerializeField] Vector3 lookingDirectionWMouse;
    [SerializeField] float rotationSpeed =250f;

    [Header("Sword Attack Variables")]
    [SerializeField] GameObject sword;
    [SerializeField] float swordAttackTime=0.4f;
    [SerializeField] bool canAttackWithSword;
    Collider swordCollider;

    [Header("Bow Attack Variables")]
    [SerializeField] GameObject bow;
    [SerializeField] GameObject arrow; // yay saldrısı sonrasında instantiate edilecek.
    [SerializeField] Transform arrowInstantiatePos;
    [SerializeField] float bowShootTime = 0.4f;
    [SerializeField] float bowShootCooldown = 1f;
    [SerializeField] float arrowShootForce;
    [SerializeField] bool canAttackWithBow;

    

    private void Awake()
    {
        playerInputs = new ThirdPersonPlayerMovement();
        playerAnimator = GetComponent<Animator>();
        swordCollider = sword.GetComponent<BoxCollider>();
        swordCollider.enabled = false; // saldırı sırasında aktif olacak

        swordAttackAction = playerInputs.Combat.SwordAttack;

        playerInputs.Combat.BowAttack.started += DrawBow; // sağ tuşa basıldığında ve basılı tutulduğunda BowDraw animasyonu gerçekleşecek
        playerInputs.Combat.BowAttack.canceled += BowShoot; // sağ tuş bırakıldığında BowShoot animasyonu gerçekleşecek

        StatusOfAttacks(true, true); // başlangıçta her iki saldırı türüde yapılabilir.
        ActivnesOfWepons(true, false); // başlangıçta kılıç görünür yay görünmez olmalıdır
    }

    void FixedUpdate()
    {
        CurrentLookingDirection();

        if ( (Input.GetMouseButton(0) && canAttackWithSword) || (Input.GetMouseButton(1) && canAttackWithBow))
        {
            LookToMouseDirection();
        }
 

        if (swordAttackAction.IsPressed() && canAttackWithSword) // eğer mouse sol tuşa basıldıysa ve player sword attack yapabilecek durumdaysa çalışır
        {
            UseSword();
        }
    }

    void CurrentLookingDirection() // mouse pozisyonunu alır ve karakterin pozisyonuna göre güncel yönü bulur
    {
        // 1. Mouse pozisyonunu al
        mousePosition = Input.mousePosition;

        // 2. Mouse'un dünya koordinatlarını al (Ortoğrafik kamera için)
        mousePosToWievPos = Camera.main.ScreenToViewportPoint(mousePosition);
        playerPosToWievPos = Camera.main.WorldToViewportPoint(transform.position);

        // mouse konumunu karakterin pozisyonundan çıkararak yönü bul  
        lookingDirectionWMouse = new Vector3(mousePosToWievPos.x - playerPosToWievPos.x, 0, mousePosToWievPos.y - playerPosToWievPos.y);
    }
    private void LookToMouseDirection() // mouse a basılı olduğu sürece ana karakteri o yöne döndürür
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookingDirectionWMouse);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    } 

    void DrawBow(InputAction.CallbackContext context) // yay çekme animasyonunu tetikler
    {
        if (canAttackWithBow)
        {
            StatusOfAttacks(false, true); // yay gerili durumda kılıca geçiş yapılmaz 
            ActivnesOfWepons(false, true); // yay ile saldırı olacağı için yayı görünür kılıcı görünmez kılıyor

            gameObject.GetComponent<PlayerMovementManager>().enabled = false; // karakter hareketini durduruyoruz
            playerAnimator.SetBool("isCharging", true); // animasyona geçiş
        }
    }

    void BowShoot(InputAction.CallbackContext context) // yay bırakma animasyonunu tetikler
    {
        if (canAttackWithBow) // eğer yay kullanılamaz durumda mouse sağ tuşa basıldıysa çalışmaz
        {
            StatusOfAttacks(false, false); // yay bırakıldıktan bir süre sonra yeni saldırı yapılabilecektir

            playerAnimator.SetBool("isCharging", false); // Yay çekme animasyonunu durdur
            playerAnimator.SetTrigger("BowAttack"); // Ok atma animasyonunu tetikle          

            // ok atma işlemi için arrow prefabı instantiate ediliyor. rigidbody sine kuvvet uygulanıyor. 
            GameObject arrowReferance = Instantiate(arrow, arrowInstantiatePos.position, Quaternion.identity);
            arrowReferance.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * arrowShootForce * Time.deltaTime, ForceMode.Impulse); // player objesinin doğrultusunda kuvvet uygulanır
            if (arrowReferance != null) // eğer arrow objesi yokolmamışsa 1 sn sonra yoket
            {
                Destroy(arrowReferance, 1f);
            }

            Invoke("AfterBowShoot", bowShootTime);
        }
    }

    void AfterBowShoot() // ok ateşlendikten sonra kılıcı görünür kılıp yayı gizler. ve harekete izin verir 
    {
        playerAnimator.ResetTrigger("BowAttack"); // Ok atma animasyonunu sıfırla
        gameObject.GetComponent<PlayerMovementManager>().enabled = true; // animasyon bittikten sonra oyuncu tekrar hareket edebilir.
        StatusOfAttacks(true, false); // yeni saldırı yapılabilir
        Invoke("BowAttackCooldownFinis",bowShootCooldown - bowShootTime);
    }

    void BowAttackCooldownFinis() // yay ile saldırı yapma durumu aktif
    {
        canAttackWithBow = true; 
    }

    void UseSword() // kılıç kullanımını tetikler
    {
        StatusOfAttacks(false, false); // ardarda saldırı tuşuna basılınca animasyon başa sarmasın diye yeni saldırı engelleniyor 
        ActivnesOfWepons(true, false); // kılıç ile saldırı olacağı için kılıcı görünür yayı görünmez kılıyor

        gameObject.GetComponent<PlayerMovementManager>().enabled = false; // karakter hareketini durduruyoruz
        StartCoroutine(PerformSwordAttack());
    }

    IEnumerator PerformSwordAttack() // kılıç ile saldırı sırasında gerçekleşen fonksiyon
    {
        swordCollider.enabled = true; // düşmana hasar verebilmesi için collider aktif edildi
        playerAnimator.SetTrigger("SwordAttack");
        yield return new WaitForSeconds(swordAttackTime); // animasyon süresi kadar bekleme gerçekleşiyor
        gameObject.GetComponent<PlayerMovementManager>().enabled = true; // animasyon bittikten sonra karakter hareketi devam edebilir
        swordCollider.enabled = false; // saldırı olmadığı esnada kılıç düşmana değerse hasar vermesin diye collider devre dışı bırakıldı
        StatusOfAttacks(true, true); // kılıç ile saldırı animasyonu bittikten sonra yeni saldırılar gerçekleştirilebilir
    }
  
    void ActivnesOfWepons(bool sword_statu,bool bow_statu) // kılıç ve yay kullanımına göre silahların görünürlüğünü ayarlar
    {
        sword.SetActive(sword_statu);
        bow.SetActive(bow_statu);
    }

    void StatusOfAttacks(bool sword_statu, bool bow_statu) // girdilere göre hangi saldırının gerçekleştirilebileceğini belirler
    {
        canAttackWithSword=sword_statu;
        canAttackWithBow = bow_statu;
    }

    private void OnEnable()
    {
        playerInputs.Combat.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Combat.Disable();
    }

}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// Этот класс определяет, какое поведение игрока является активным или переопределяющим, и вызывает его локальные функции.
// Содержит базовую настройку и общие функции, используемые всем поведением игрока.
public class BasicBehaviour : MonoBehaviour
{
    public Transform playerCamera;                        // Ссылка на камеру, которая фокусируетcя на игрока.
    public float turnSmoothing = 0.06f;                   // Скорость поворота при движении по направлению камеры.
    public float sprintFOV = 100f;                        // Поле зрения камеры во время бега.
    public string sprintButton = "Sprint";                // Название кнопки для бега.
    public float deltaStamina = 15f;
    public Slider slider;
    public float time = 60f;
    public Text timer;

    private float h;                                      // Горизонтальная ось.
    private float v;                                      // Вертикальная ось.
    private int currentBehaviour;                         // Ссылка на текущее поведение игрока.
    private int defaultBehaviour;                         // Поведение игрока по умолчанию
    private int behaviourLocked;                          // Ссылка на временно заблокированное поведение, которое нельзя изменять.
    private Vector3 lastDirection;                        // Последнее направление, в котором двигался игрок.
    private Animator anim;                                // Ссылка на компонент Animator.
    private ThirdPersonOrbitCamBasic camScript;           // Ссылка на скрипт управления камерой от 3-го лица.
    private bool sprint;                                  // Булевое значение, которое определяет, бежит игрок или нет.
    private bool hasStamina = true;                       // Булевое значение, которое определяет, восстанавливается выносливость, или нет.
    private bool changedFOV;                              // Булевое значение, которое определяет изменинение поля зрения при беге.
    private int hFloat;                                   // Переменная аниматора, связанная с горизонтальной осью.
    private int vFloat;                                   // Переменная аниматора, связанная с мертикальной осью.
    private List<GenericBehaviour> behaviours;            // Список, содержащий все доступные поведения игрока.
    private List<GenericBehaviour> overridingBehaviours;  // Список текущего преоблащающего поведения.
    private Rigidbody rBody;                              // Ссылка на rigidbody игрока.
    private int groundedBool;                             // Булевая переменная аниматора, которая определяет на земле игрок, или нет.
    private Vector3 colExtents;                           // Коллайдер для проверки столкновения с землей. 
    private float stamina = 100f;
    private bool isGround = true;

    // Получение текущих горизонтальных и вертикальных осей.
    public float GetH { get { return h; } }
    public float GetV { get { return v; } }

    // Получение скрипта управления камерой.
    public ThirdPersonOrbitCamBasic GetCamScript { get { return camScript; } }

    // Получение rigidbody игрока.
    public Rigidbody GetRigidBody { get { return rBody; } }

    // Получение контроллера анимации игрока.
    public Animator GetAnim { get { return anim; } }

    // Получение текущего поведения по умолчанию.
    public int GetDefaultBehaviour { get { return defaultBehaviour; } }

    void Awake()
    {
        // Установка ссылок.
        behaviours = new List<GenericBehaviour>();
        overridingBehaviours = new List<GenericBehaviour>();
        anim = GetComponent<Animator>();
        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");
        camScript = playerCamera.GetComponent<ThirdPersonOrbitCamBasic>();
        rBody = GetComponent<Rigidbody>();


        // Переменные для проверки нахождения игрока на земле.
        groundedBool = Animator.StringToHash("Grounded");
        colExtents = GetComponent<Collider>().bounds.extents;
    }

    void Update()
    {
        // Сохранение осей ввода.
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        // Установка осей ввода на контроллер анимации.
        anim.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        anim.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        // Переключения бега по нажатию.
        if (stamina > 0.0f)
        {
            sprint = Input.GetButton(sprintButton);
        }
        else
        {
            hasStamina = false;
            sprint = false;
        }


        // Установка корректного поля зрения камеры для бега.
        if (IsSprinting())
        {
            changedFOV = true;
            camScript.SetFOV(sprintFOV);
            stamina -= deltaStamina * Time.deltaTime;
        }
        else if (changedFOV)
        {
            camScript.ResetFOV();
            changedFOV = false;
        }
        else if (stamina < 100f)
        {
            stamina += deltaStamina * Time.deltaTime / 2;
        }

        if (Input.GetButtonUp(sprintButton))
        {
            hasStamina = true;
        }

        slider.value = stamina;

        // Установка на контроллер анимации проверки на столкновение с землёй.
        anim.SetBool(groundedBool, isGround);

        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = 0;
        }

        timer.text = FloatToTimeString(Mathf.Round(time));
    }

    // Вызов функции активного или переопределяющее поведения.
    void FixedUpdate()
    {
        // Активное поведение, если переопределяющее отсутвует.
        bool isAnyBehaviourActive = false;
        if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
        {
            foreach (GenericBehaviour behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
                {
                    isAnyBehaviourActive = true;
                    behaviour.LocalFixedUpdate();
                }
            }
        }
        // Переопределяющее поведение.
        else
        {
            foreach (GenericBehaviour behaviour in overridingBehaviours)
            {
                behaviour.LocalFixedUpdate();
            }
        }

        // Проверка на то, что игрок будет стоять на земле, если никакое поведение не является активным или переопределяющим.
        if (!isAnyBehaviourActive && overridingBehaviours.Count == 0)
        {
            rBody.useGravity = true;
            Repositioning();
        }
        SetGrounded();
    }

    // Вызов функций активного или переопределяющего поведения.
    private void LateUpdate()
    {
        // Активное поведение, если переопределяющее отсутвует.
        if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
        {
            foreach (GenericBehaviour behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }
        // Переопределяющее поведение.
        else
        {
            foreach (GenericBehaviour behaviour in overridingBehaviours)
            {
                behaviour.LocalLateUpdate();
            }
        }
    }

    // Добавление отслеживания нового поведения.
    public void SubscribeBehaviour(GenericBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }

    // Установка поведения по умолчанию.
    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }

    // Установка нового поведения.
    public void RegisterBehaviour(int behaviourCode)
    {
        if (currentBehaviour == defaultBehaviour)
        {
            currentBehaviour = behaviourCode;
        }
    }

    // Установка поведения по умолчанию.
    public void UnregisterBehaviour(int behaviourCode)
    {
        if (currentBehaviour == behaviourCode)
        {
            currentBehaviour = defaultBehaviour;
        }
    }

    // Переопределение любого активного поведения поведением в очереди.
    public bool OverrideWithBehaviour(GenericBehaviour behaviour)
    {
        // Поведение отсутсвует в очереди.
        if (!overridingBehaviours.Contains(behaviour))
        {
            // Если никакое нестандартное поведение в настоящее время не используется.
            if (overridingBehaviours.Count == 0)
            {
                // Вызов метода OnOverride прежде чем добавить в очередь.
                foreach (GenericBehaviour overriddenBehaviour in behaviours)
                {
                    if (overriddenBehaviour.isActiveAndEnabled && currentBehaviour == overriddenBehaviour.GetBehaviourCode())
                    {
                        overriddenBehaviour.OnOverride();
                        break;
                    }
                }
            }
            // Добавление нового поведения в очередь.
            overridingBehaviours.Add(behaviour);
            return true;
        }
        return false;
    }

    // Попытка отменить переопределяющее поведение и вернуться к активному.
    // Вызывается при выходе из режима переопределения.
    public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
    {
        if (overridingBehaviours.Contains(behaviour))
        {
            overridingBehaviours.Remove(behaviour);
            return true;
        }
        return false;
    }

    // Проверка, не отменяет ли какое-либо конкретное поведение активное.
    public bool IsOverriding(GenericBehaviour behaviour = null)
    {
        if (behaviour == null)
        {
            return overridingBehaviours.Count > 0;
        }
        return overridingBehaviours.Contains(behaviour);
    }

    // Проверка на соответсвие текущему поведению.
    public bool IsCurrentBehaviour(int behaviourCode)
    {
        return this.currentBehaviour == behaviourCode;
    }

    // Проверка, не заблокировано ли какое-либо поведение.
    public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
    {
        return (behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf);
    }

    // Попытка заблокировать определенное поведение.
    // Никакое другое поведение не может быть использовано во время временной блокировки.
    // Используется для временных переходов, таких как прыжки и т.д.
    public void LockTempBehaviour(int behaviourCode)
    {
        if (behaviourLocked == 0)
        {
            behaviourLocked = behaviourCode;
        }
    }

    // Разблокировака текущего заблокированного поведения.
    // Используется после окончания временного перехода.
    public void UnlockTempBehaviour(int behaviourCode)
    {
        if (behaviourLocked == behaviourCode)
        {
            behaviourLocked = 0;
        }
    }

    // Проверка на процесс бега.
    public virtual bool IsSprinting()
    {
        return sprint && IsMoving() && CanSprint() && hasStamina;
    }

    // Проверка на возможность бега.
    public bool CanSprint()
    {
        foreach (GenericBehaviour behaviour in behaviours)
        {
            if (!behaviour.AllowSprint())
                return false;
        }
        foreach (GenericBehaviour behaviour in overridingBehaviours)
        {
            if (!behaviour.AllowSprint())
                return false;
        }
        return true;
    }

    // Проверка на движение в горизонтальной плоскости.
    public bool IsHorizontalMoving()
    {
        return h != 0;
    }

    // Проверка на движение игрока.
    public bool IsMoving()
    {
        return (h != 0) || (v != 0);
    }

    // Получение последнего направления движения.
    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }

    // Установка последнего направления движения.
    public void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }

    // Игрок в положение стоя, исходя из последнего направления движения.
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmoothing);
            rBody.MoveRotation(newRotation);
        }
    }

    // Проверка, находится человек на земле или нет.
    public bool IsGrounded()
    {
        return isGround;
    }

    private void SetGrounded()
    {
        Ray ray = new Ray(this.transform.position + 2 * Vector3.up * colExtents.x, Vector3.down);
        isGround = Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.25f);
    }

    private string FloatToTimeString(float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = time - minutes * 60;

        return minutes.ToString() + ":" + (seconds < 10 ? "0" : "") + seconds.ToString();
    }
}

// Это базовый класс для любого поведения игрока, любое настраиваемое поведение должно унаследоваться от него.
// Содержит ссылки на локальные компоненты, которые могут отличаться в зависимости от самого поведения.
public abstract class GenericBehaviour : MonoBehaviour
{
    //protected Animator anim;                       // Reference to the Animator component.
    protected int speedFloat;                      // Скорость анимации.
    protected BasicBehaviour behaviourManager;     // Ссылка на базовый менеджер поведения.
    protected int behaviourCode;                   // Код идентификации.
    protected bool canSprint;                      // Булевое значение, которое определяет, может бежать игрок или нет.

    void Awake()
    {
        // Установка ссылок.
        behaviourManager = GetComponent<BasicBehaviour>();
        speedFloat = Animator.StringToHash("Speed");
        canSprint = true;

        // Установка кода на основе наследуемого класса.
        behaviourCode = this.GetType().GetHashCode();
    }

    // Поведение будет управлять действиями игрока с помощью следующих функций:

    // Локальный эквивалент функции FixedUpdate в MonoBehaviour.
    public virtual void LocalFixedUpdate() { }
    // Локальный эквивалент функции LateUpdate в MonoBehaviour.
    public virtual void LocalLateUpdate() { }
    // Эта функция вызывается, когда другое поведение заменяет текущее.
    public virtual void OnOverride() { }

    // Получение кода.
    public int GetBehaviourCode()
    {
        return behaviourCode;
    }

    // Проверка, позволяет ли поведение спринт.
    public bool AllowSprint()
    {
        return canSprint;
    }
}
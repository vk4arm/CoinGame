using UnityEngine;


// MoveBehaviour наследуется от GenericBehaviour. Этот класс соответствует базовому поведению ходьбы и бега, это поведение по умолчанию.
public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f;                 // Скорость хотьбы по умолчанию.
    public float runSpeed = 1.0f;                   // Скорость бега по умолчанию.
    public float sprintSpeed = 2.0f;                // Скорость спринта по умолчанию.
    public float speedDampTime = 0.1f;              // Время сглаживания на основании текущей скорости, чтобы изменить анимацию.
    public string jumpButton = "Jump";              // Кнопка прыжка по умолчанию.
    public float jumpHeight = 1.5f;                 // Высота прыжка по умолчанию.
    public float jumpIntertialForce = 10f;          // Горизонтальная сила прыжка по умолчанию.

    private float speed;                            // Скорость движения.
    private int jumpBool;                           // Переменная аниматора, ответственная за прыжок.
    private int groundedBool;                       // Переменная аниматора, ответственная за нахождение на земле.
    private bool jump;                              // Булевое значение, определяющее прыжок.
    private bool isColliding;                       // Булевое значения, определяющее коллизию.
    private ContactPoint LastContactPoint;          // Последняя точка контакта при коллизии.

    // Start вызывается после любых функций Awake.
    void Start()
    {
        // Установка ссылок.
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        behaviourManager.GetAnim.SetBool(groundedBool, true);

        // Установка поведения по умолчанию.
        behaviourManager.SubscribeBehaviour(this);
        behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);
    }

    // Update используется для установки параметров вне зависимости от текущего поведения.
    void Update()
    {
        // Get jump input.
        if (!jump && Input.GetButtonDown(jumpButton) && behaviourManager.IsCurrentBehaviour(this.behaviourCode) && !behaviourManager.IsOverriding())
        {
            jump = true;
        }
    }

    // LocalFixedUpdate переопределяет виртуальную функцию родительского класса.
    public override void LocalFixedUpdate()
    {
        // Вызов базового менеджера движения.
        MovementManagement(behaviourManager.GetH, behaviourManager.GetV);

        // Вызов менеджера прыжка.
        JumpManagement();
    }

    // Движение прыжка.
    void JumpManagement()
    {
        // Начало прыжка.
        if (jump && !behaviourManager.GetAnim.GetBool(jumpBool) && behaviourManager.IsGrounded())
        {
            // Установка параметров перехода.
            behaviourManager.LockTempBehaviour(this.behaviourCode);
            behaviourManager.GetAnim.SetBool(jumpBool, true);
            // Является ли передвижение прыжком?
            if (behaviourManager.GetAnim.GetFloat(speedFloat) > 0)
            {
                // Временно изменяем трение игрока.
                GetComponent<Collider>().material.dynamicFriction = 0f;
                GetComponent<Collider>().material.staticFriction = 0f;
                // Убираем вертикальную скорость, чтобы избежать "супер-прыжков" в конце.
                RemoveVerticalVelocity();

                // Устанавливаем скорость вертикального импульса прыжка.
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                behaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
            }
        }
        // Прыжок завершен?
        else if (behaviourManager.GetAnim.GetBool(jumpBool))
        {
            // Продолжаем движение вперед в воздухе.
            if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
            {
                behaviourManager.GetRigidBody.AddForce(transform.forward * jumpIntertialForce * Physics.gravity.magnitude * Mathf.Clamp(speed, 0, sprintSpeed - 1), ForceMode.Acceleration);
            }
            // Игрок находится на земле?
            if ((behaviourManager.GetRigidBody.velocity.y < 0) && behaviourManager.IsGrounded())
            {
                behaviourManager.GetAnim.SetBool(groundedBool, true);
                // Установка трения по умолчанию.
                GetComponent<Collider>().material.dynamicFriction = 0.6f;
                GetComponent<Collider>().material.staticFriction = 0.6f;
                // Установка параметров перехода.
                jump = false;
                behaviourManager.GetAnim.SetBool(jumpBool, false);
                behaviourManager.UnlockTempBehaviour(this.behaviourCode);
            }
        }
    }

    // Основное движение.
    void MovementManagement(float horizontal, float vertical)
    {
        // На земле включается гравитация.
        if (behaviourManager.IsGrounded())
            behaviourManager.GetRigidBody.useGravity = true;

        // Отключение вертикального движения при достижении земли.
        if (!behaviourManager.GetAnim.GetBool(jumpBool) && behaviourManager.GetRigidBody.velocity.y > 0)
        {
            RemoveVerticalVelocity();
        }

        // Вызов функции, которая определеяет ориентацию игрока.
        Rotating(horizontal, vertical);

        // Устанавка правильной скорости.
        Vector2 dir = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        speed = Vector2.ClampMagnitude(dir, runSpeed).magnitude;

        if (behaviourManager.IsSprinting())
        {
            speed = sprintSpeed;
        }

        behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }

    // Удаление вертикальной скорости.
    private void RemoveVerticalVelocity()
    {
        Vector3 horizontalVelocity = behaviourManager.GetRigidBody.velocity;
        horizontalVelocity.y = 0;
        behaviourManager.GetRigidBody.velocity = horizontalVelocity;
    }

    // Поворот игрока, чтобы он соответствовал правильной ориентации, в зависимости от камеры и нажатой клавиши.
    Vector3 Rotating(float horizontal, float vertical)
    {
        // Получение направления камеры без вертикальной составляющей.
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

        // Игрок движется по земле, Y-компонента ориентации камеры не имеет значения.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Вычисление направления на основе направления камеры и клавиши движения.
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection;
        targetDirection = forward * vertical + right * horizontal;

        // Lerp механизм движения.
        if ((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);
            behaviourManager.GetRigidBody.MoveRotation(newRotation);
            behaviourManager.SetLastDirection(targetDirection);
        }

        // В режиме ожидания игноририрование текущей ориентации камеры и учет последнего направления движения.
        if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
        {
            behaviourManager.Repositioning();
        }

        return targetDirection;
    }

    // Определение коллизий.
    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        foreach (ContactPoint Contact in collision.contacts)
        {
            LastContactPoint = Contact;
        }
        // Скольжение
        if (behaviourManager.IsCurrentBehaviour(this.GetBehaviourCode()) && collision.GetContact(0).normal.y <= 0.1f)
        {
            GetComponent<Collider>().material.dynamicFriction = 0f;
            GetComponent<Collider>().material.staticFriction = 0f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (LastContactPoint.normal.y >= 0.8f)
        {
            isColliding = false;
        }
        GetComponent<Collider>().material.dynamicFriction = 0.6f;
        GetComponent<Collider>().material.staticFriction = 0.6f;
    }
}


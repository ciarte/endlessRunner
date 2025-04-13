using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DireccionInput
{
    Null,
    Arriba,
    Abajo, Izquierda,
    Derecha
}


public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float valorSalto = 15f;
    [SerializeField] private float gravedad = 20f;

    [Header("Carril")]
    [SerializeField] private float posicionCarrilIzquierdo = -3.1f;
    [SerializeField] private float posicionCarrilDerecho = 3.1f;

    public bool EstaSaltando { get; private set; }
    public bool EstaDeslizando { get; private set; }
    private DireccionInput direccionInput;
    private CharacterController characterController;
    private PlayerAnimation playerAnimation;
    private float posicionVertical;
    private int carrilActual;
    private Vector3 direccionDeseada;
    //Variables para el CharacterController
    private float controllerRadio;
    private float controllerAltura;
    private float controllerPosicionY;
    private Coroutine coroutineDeslizar;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }


    void Start()
    {
        controllerRadio = characterController.radius;
        controllerAltura = characterController.height;
        controllerPosicionY = characterController.center.y;
    }
    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.EstadoActual == EstadosDelJuego.Inicio || GameManager.Instance.EstadoActual == EstadosDelJuego.GameOver) return;

        DetectarInput();
        ControlarCarriles();
        CalcularMovimientoVertical();
        MoverPersonaje();
    }


    private void MoverPersonaje()
    {
        Vector3 nuevaPosicion = new Vector3(direccionDeseada.x, posicionVertical, velocidadMovimiento);
        characterController.Move(nuevaPosicion * Time.deltaTime);
    }

    private void CalcularMovimientoVertical()
    {
        if (characterController.isGrounded)
        {
            EstaSaltando = false;
            posicionVertical = 0f;
            if (EstaDeslizando == false && EstaSaltando == false)
            {
                playerAnimation.MostrarAnimacionCorrer();
            }
            if (direccionInput == DireccionInput.Arriba)
            {
                playerAnimation.MostrarAnimacionSaltar();
                EstaSaltando = true;
                posicionVertical = valorSalto;
                if (coroutineDeslizar != null)
                {
                    StopCoroutine(coroutineDeslizar);
                    EstaDeslizando = false;
                    ModificarColliderDeslizar(false);
                }
            }
            if (direccionInput == DireccionInput.Abajo)
            {
                if (EstaDeslizando) return;
                if (coroutineDeslizar != null)
                {
                    StopCoroutine(coroutineDeslizar);

                }
                DeslizarPersonaje();

            }

        }
        else
        {

            if (direccionInput == DireccionInput.Abajo)
            {
                posicionVertical -= valorSalto;
                DeslizarPersonaje();
            }
        }
        posicionVertical -= gravedad * Time.deltaTime;

    }

    private void ControlarCarriles()
    {

        switch (carrilActual)
        {
            case -1:
                LogicaCarrilIzquierdo();
                break;
            case 0:
                LogicaCarrilCentral();
                break;
            case 1:
                LogicaCarrilDerecho();
                break;
        }
    }

    private void LogicaCarrilIzquierdo()
    {
        MoverHorizontal(posicionCarrilIzquierdo, Vector3.left);
    }
    private void LogicaCarrilDerecho()
    {
        MoverHorizontal(posicionCarrilDerecho, Vector3.right);
    }

    private void LogicaCarrilCentral()
    {
        if (transform.position.x > 0.1f)
        {
            MoverHorizontal(0, Vector3.left);
        }
        else if (transform.position.x < -0.1f)
        {
            MoverHorizontal(0, Vector3.right);
        }
        else
        {
            direccionDeseada = Vector3.zero;
        }
    }

    //MOVIMIENTO ALOS LADOS   
    //Lerp para suavizar el movimiento a los lados
    //Vector3.Lerp: Interpola entre dos vectores, en este caso entre la posicion actual y la posicion deseada.
    //Mathf.Abs: Devuelve el valor absoluto de un número, en este caso la distancia entre la posicion actual y la posicion deseada.
    private void MoverHorizontal(float posicionX, Vector3 dirMovimiento)
    {

        float posicionHorizontal = Mathf.Abs(transform.position.x - posicionX);
        if (posicionHorizontal > 0.1f)
        {
            direccionDeseada = Vector3.Lerp(direccionDeseada, dirMovimiento * 20f, Time.deltaTime * 50f);
        }
        else
        {
            direccionDeseada = Vector3.zero;
            transform.position = new Vector3(posicionX, transform.position.y, transform.position.z);

        }

    }


    private void DeslizarPersonaje()
    {
        coroutineDeslizar = StartCoroutine(CODeslizarPersonaje());
    }

    private IEnumerator CODeslizarPersonaje()
    {
        EstaDeslizando = true;
        ModificarColliderDeslizar(EstaDeslizando);
        playerAnimation.MostrarAnimacionDeslizar();
        yield return new WaitForSeconds(1.5f);
        EstaDeslizando = false;
        ModificarColliderDeslizar(EstaDeslizando);
        playerAnimation.MostrarAnimacionCorrer();
    }


    private void ModificarColliderDeslizar(bool estaDeslizando)
    {
        if (estaDeslizando)
        {
            characterController.radius = 0.3f;
            characterController.height = 0.6f;
            characterController.center = new Vector3(0, 0.35f, 0);

        }
        else
        {

            characterController.radius = controllerRadio;
            characterController.height = controllerAltura;
            characterController.center = new Vector3(0f, controllerPosicionY, 0f);
        }
    }
    void DetectarInput()
    {
        direccionInput = DireccionInput.Null;
        if (Input.GetKeyDown(KeyCode.A))
        {
            direccionInput = DireccionInput.Izquierda;
            carrilActual--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direccionInput = DireccionInput.Derecha;
            carrilActual++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            direccionInput = DireccionInput.Arriba;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direccionInput = DireccionInput.Abajo;
        }


        carrilActual = Mathf.Clamp(carrilActual, -1, 1);

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstaculo"))
        {

            if (GameManager.Instance.EstadoActual == EstadosDelJuego.GameOver) return;
            // Si el jugador colisiona con un obstáculo, se detiene el juego y se muestra la animación de colisión
            playerAnimation.MostrarAnimacionColision();
            GameManager.Instance.CambiarEstado(EstadosDelJuego.GameOver);
        }
    }

}

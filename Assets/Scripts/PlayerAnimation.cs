using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private Animator animator;
    private string animacionActual;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void CambiarAnimacion(string nuevaAnimacion)
    {
        if (animacionActual == nuevaAnimacion) return;
        // Si la animación actual es diferente a la nueva animación, la cambiamos

        animator.Play(nuevaAnimacion);
        animacionActual = nuevaAnimacion;

    }

    public void MostrarAnimacionIdle()
    {

        CambiarAnimacion("Idle");
    }
    public void MostrarAnimacionCorrer()
    {

        CambiarAnimacion("Run");
    }
    public void MostrarAnimacionSaltar()
    {

        CambiarAnimacion("Jump");
    }
    public void MostrarAnimacionDeslizar()
    {

        CambiarAnimacion("Crawl");
    }
    public void MostrarAnimacionColision()
    {

        CambiarAnimacion("Dead");
    }

}
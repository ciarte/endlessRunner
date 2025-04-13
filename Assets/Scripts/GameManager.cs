using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum EstadosDelJuego
{
    Inicio,
    Jugando,
    Pausado,
    GameOver
}
public class GameManager : Singleton<GameManager>
{


    public EstadosDelJuego EstadoActual { get; private set; }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CambiarEstado(EstadosDelJuego.Jugando);
        }
    }

    public void CambiarEstado(EstadosDelJuego nuevoEstado)
    {

        if (EstadoActual != nuevoEstado)
        {
            EstadoActual = nuevoEstado;
        }
    }
}

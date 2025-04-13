using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] float countdownTime = 300f;
    bool isTimmerRuning = true;
    float currentTime;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countdownText; // Texto de cuenta atrás en pantalla

    //Referencias a los LifeManager y HitDetector de los jugadores
    public LifeManager player1LifeManager;
    public LifeManager player2LifeManager;
    public HitDetector player1HitDetector;
    public HitDetector player2HitDetector;

    private void Start()
    {
        currentTime = countdownTime;
        StartCoroutine(PreMatchCountdown());
    }

    IEnumerator PreMatchCountdown()
    {
        Time.timeScale = 0f;

        int count = 3;

        while(count > 0)
        {
            countdownText.text = count.ToString();
            yield return StartCoroutine(WaitForRealSeconds(1f));
            count--;
        }

        countdownText.text = "GO!";
        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownText.text = "";
        Time.timeScale = 1f; //Reanuda el tiempo del jugador
        isTimmerRuning = true;
        StartCoroutine(UpdateTimer());

    }

    //Espera real que ignora Time.timeScale = 0
    private IEnumerator WaitForRealSeconds(float seconds)
    {
        float satrt = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < satrt + seconds)
        {
            yield return null;
        }
    }

    // Corutina que actualiza el temporizador
    private IEnumerator UpdateTimer()
    {
        while (currentTime > 0 && isTimmerRuning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            yield return null; //Esperamos un frame.
        }

        //Cuando el tiempo llega a 0 
        isTimmerRuning = false;
        DetermineWinner();
    }

    //Metodo para actualizar el texto del temporizador en la UI
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    //Metodo para determinar el ganador
    private void DetermineWinner()
    {
        //Acceso a las vidas y porcentajes de cada jugador usando el booleano isPlayerOne
        int player1Lives = player1LifeManager.GetLives();
        int player2Lives = player2LifeManager.GetLives();

        //Si las vidas no son iguales, el jugador con más vidas gana
        if(player1Lives > player2Lives)
        {
            Debug.Log("Jugador 1 gana por más vidas!");
        }
        else if (player2Lives > player1Lives)
        {
            Debug.Log("Jugador 2 gana por más vidas");
        }
        else
        {
            //Si las vidas son iguales, se compara el porcentaje de daño
            if (player1HitDetector.damagePercentage < player2HitDetector.damagePercentage)
            {
                Debug.Log("Jugador gana por menor porcentaje de Muerte");
            }
            else if (player2HitDetector.damagePercentage < player1HitDetector.damagePercentage)
            {
                Debug.Log("Jugador gana por menor porcentaje de Muerte");
            }
            else
            {
                Debug.Log("Empate! Ambos jugadores tienen el mismo porcentaje de Muerte");
            }
        }
        Time.timeScale = 0f;
    }

}
